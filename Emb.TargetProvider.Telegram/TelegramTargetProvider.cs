using Emb.Common.Abstractions;
using Emb.TargetProvider.Telegram.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Composition;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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

            await telegramBotClient.SendTextMessageAsync(new ChatId(endpointOptionsString), text);
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
