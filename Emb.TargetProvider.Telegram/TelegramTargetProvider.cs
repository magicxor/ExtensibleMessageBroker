using Emb.Common.Abstractions;
using Emb.TargetProvider.Telegram.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Emb.TargetProvider.Telegram
{
    [Export(typeof(ITargetProvider))]
    public class TelegramTargetProvider : ITargetProvider
    {
        private readonly TelegramBotClientFactory _telegramBotClientFactory = new TelegramBotClientFactory();
        
        public async Task SendAsync(ILoggerFactory loggerFactory, 
            IConfigurationRoot configurationRoot, 
            string endpointOptionsString, 
            string text,
            CancellationToken cancellationToken)
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
                        logger.LogDebug("{TelegramTargetProviderName}.{SendAsyncName} attempt {Attempt}", nameof(TelegramTargetProvider), nameof(SendAsync), attempt);

                        await telegramBotClient.SendMessage(new ChatId(endpointOptionsString), text, cancellationToken: cancellationToken);

                        if (providerSettings.DelayMilliseconds > 0)
                        {
                            await Task.Delay(providerSettings.DelayMilliseconds, cancellationToken);
                        }
                    }
                    catch(Exception e)
                    {
                        logger.LogTrace(e, "{SendAsyncName} error", nameof(SendAsync));
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
