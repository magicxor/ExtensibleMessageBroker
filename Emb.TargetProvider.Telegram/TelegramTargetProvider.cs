using System;
using System.Composition;
using System.Net;
using System.Threading.Tasks;
using Emb.Common.Abstractions;
using Emb.TargetProvider.Telegram.Enums;
using Emb.TargetProvider.Telegram.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Emb.TargetProvider.Telegram
{
    [Export(typeof(ITargetProvider))]
    public class TelegramTargetProvider : ITargetProvider
    {
        private ITelegramBotClient CreateTelegramBotClient(ProviderSettings providerSettings)
        {
            IWebProxy webProxy = null;
            switch (providerSettings.ProxyProtocol)
            {
                case ProxyProtocols.None:
                    break;
                case ProxyProtocols.Http:
                    var proxyUriHttp = new UriBuilder("http", providerSettings.ProxyHost, providerSettings.ProxyPort).Uri;
                    if (providerSettings.ProxyAuthentication)
                    {
                        var credentialsHttp = new NetworkCredential(providerSettings.ProxyUsername, providerSettings.ProxyPassword);
                        webProxy = new WebProxy(proxyUriHttp, true, null, credentialsHttp);
                    }
                    else
                    {
                        webProxy = new WebProxy(proxyUriHttp);
                    }
                    break;
                case ProxyProtocols.Socks5:
                    HttpToSocks5Proxy httpToSocks5Proxy;
                    if (providerSettings.ProxyAuthentication)
                    {
                        httpToSocks5Proxy = new HttpToSocks5Proxy(providerSettings.ProxyHost, providerSettings.ProxyPort, providerSettings.ProxyUsername, providerSettings.ProxyPassword);
                    }
                    else
                    {
                        httpToSocks5Proxy = new HttpToSocks5Proxy(providerSettings.ProxyHost, providerSettings.ProxyPort);
                    }
                    webProxy = httpToSocks5Proxy;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            TelegramBotClient telegramBotClient;
            if (webProxy == null)
            {
                telegramBotClient = new TelegramBotClient(providerSettings.Token);
            }
            else
            {
                telegramBotClient = new TelegramBotClient(providerSettings.Token, webProxy);
            }

            return telegramBotClient;
        }

        public async Task SendAsync(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot, string endpointOptionsString, string text)
        {
            var providerSettings = configurationRoot.GetSection(GetType().Name).Get<ProviderSettings>();
            var telegramBotClient = CreateTelegramBotClient(providerSettings);

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
