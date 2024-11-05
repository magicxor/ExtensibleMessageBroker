using Emb.TargetProvider.Telegram.Enums;
using Emb.TargetProvider.Telegram.Models;
using MihaZupan;
using System;
using System.Net;
using Telegram.Bot;

namespace Emb.TargetProvider.Telegram
{
    public class TelegramBotClientFactory
    {
        public ITelegramBotClient CreateTelegramBotClient(ProviderSettings providerSettings)
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

            var telegramBotClient = new TelegramBotClient(providerSettings.Token);

            // todo: use webProxy if needed in order to circumvent Telegram blocking

            return telegramBotClient;
        }
    }
}
