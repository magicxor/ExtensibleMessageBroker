using Emb.Common.Abstractions;
using Emb.TargetProvider.Telegram.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Composition;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Emb.TargetProvider.Telegram
{
    [Export(typeof(ITargetProvider))]
    public class TelegramTargetProvider : ITargetProvider
    {
        private readonly TelegramBotClientFactory _telegramBotClientFactory = new TelegramBotClientFactory();
        
        public async Task SendAsync(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot, string endpointOptionsString, string text)
        {
            var providerSettings = configurationRoot.GetSection(GetType().Name).Get<ProviderSettings>();
            var telegramBotClient = _telegramBotClientFactory.CreateTelegramBotClient(providerSettings);
            
            const int maxTelegramStringLength = 4095;
            if (text.Length > maxTelegramStringLength)
            {
                text = text.Substring(0, maxTelegramStringLength);
            }

            var logger = loggerFactory.CreateLogger<TelegramTargetProvider>();

            var attempt = 0;
            var policyResult = await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(providerSettings.RetryCount, retryCounter => 
                {
                    switch (retryCounter)
                    {
                        case 0:
                            return TimeSpan.Zero;
                        case 1:
                            return TimeSpan.FromSeconds(1);
                        default:
                            return TimeSpan.FromMinutes(1);
                    }
                })
                .ExecuteAndCaptureAsync(async () => 
                {
                    try
                    {
                        attempt++;
                        logger.LogDebug($"{nameof(TelegramTargetProvider)}.{nameof(SendAsync)} attempt №{attempt}");

                        await telegramBotClient.SendTextMessageAsync(new ChatId(endpointOptionsString), text);

                        if (providerSettings.DelayMilliseconds > 0)
                        {
                            await Task.Delay(providerSettings.DelayMilliseconds);
                        }
                    }
                    catch(Exception e)
                    {
                        logger.LogTrace(e, $"{nameof(SendAsync)} error");
                        throw;
                    }
                });

            if (policyResult.Outcome == OutcomeType.Failure)
            {
                throw policyResult.FinalException;
            }
        }

        public Type GetEndpointOptionsType()
        {
            return typeof(string);
        }

        public Type GetProviderSettingsType()
        {
            return typeof(ProviderSettings);
        }
    }
}
