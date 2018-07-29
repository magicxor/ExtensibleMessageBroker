using System;
using System.Composition;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using com.LandonKey.SocksWebProxy;
using com.LandonKey.SocksWebProxy.Proxy;
using Emb.Common.Abstractions;
using Emb.TargetProvider.Telegram.Enums;
using Emb.TargetProvider.Telegram.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Emb.TargetProvider.Telegram
{
    [Export(typeof(ITargetProvider))]
    public class TelegramTargetProvider : ITargetProvider
    {
        private const int MaxTelegramStringLength = 4095;

        private int GetNextFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return port;
        }

        private ITelegramBotClient CreateTelegramBotClient(ProviderSettings providerSettings)
        {
            TelegramBotClient telegramBotClient;
            switch (providerSettings.ProxyProtocol)
            {
                case ProxyProtocols.None:
                    telegramBotClient = new TelegramBotClient(providerSettings.Token);
                    break;
                case ProxyProtocols.Http:
                    var proxyUriHttp = new UriBuilder("http", providerSettings.ProxyHost, providerSettings.ProxyPort).Uri;
                    if (providerSettings.ProxyAuthentication)
                    {
                        var credentialsHttp = new NetworkCredential(providerSettings.ProxyUsername, providerSettings.ProxyPassword);
                        telegramBotClient = new TelegramBotClient(providerSettings.Token, new WebProxy(proxyUriHttp, true, null, credentialsHttp));
                    }
                    else
                    {
                        telegramBotClient = new TelegramBotClient(providerSettings.Token, new WebProxy(proxyUriHttp));
                    }
                    break;
                case ProxyProtocols.Https:
                    var proxyUriHttps = new UriBuilder("https", providerSettings.ProxyHost, providerSettings.ProxyPort).Uri;
                    if (providerSettings.ProxyAuthentication)
                    {
                        var credentialsHttps = new NetworkCredential(providerSettings.ProxyUsername, providerSettings.ProxyPassword);
                        telegramBotClient = new TelegramBotClient(providerSettings.Token, new WebProxy(proxyUriHttps, true, null, credentialsHttps));
                    }
                    else
                    {
                        telegramBotClient = new TelegramBotClient(providerSettings.Token, new WebProxy(proxyUriHttps));
                    }
                    break;
                case ProxyProtocols.Socks4:
                    var hostAddressesSocks4 = Dns.GetHostAddresses(providerSettings.ProxyHost);
                    if (providerSettings.ProxyAuthentication)
                    {
                        telegramBotClient = new TelegramBotClient(providerSettings.Token, new SocksWebProxy(new ProxyConfig(
                            IPAddress.Parse("127.0.0.1"),
                            GetNextFreePort(),
                            hostAddressesSocks4.First(),
                            providerSettings.ProxyPort,
                            ProxyConfig.SocksVersion.Four,
                            providerSettings.ProxyUsername,
                            providerSettings.ProxyPassword
                        )));
                    }
                    else
                    {
                        telegramBotClient = new TelegramBotClient(providerSettings.Token, new SocksWebProxy(new ProxyConfig(
                            IPAddress.Parse("127.0.0.1"),
                            GetNextFreePort(),
                            hostAddressesSocks4.First(),
                            providerSettings.ProxyPort,
                            ProxyConfig.SocksVersion.Four
                        )));
                    }
                    break;
                case ProxyProtocols.Socks5:
                    var hostAddressesSocks5 = Dns.GetHostAddresses(providerSettings.ProxyHost);
                    if (providerSettings.ProxyAuthentication)
                    {
                        telegramBotClient = new TelegramBotClient(providerSettings.Token, new SocksWebProxy(new ProxyConfig(
                            IPAddress.Parse("127.0.0.1"),
                            GetNextFreePort(),
                            hostAddressesSocks5.First(),
                            providerSettings.ProxyPort,
                            ProxyConfig.SocksVersion.Five,
                            providerSettings.ProxyUsername,
                            providerSettings.ProxyPassword
                        )));
                    }
                    else
                    {
                        telegramBotClient = new TelegramBotClient(providerSettings.Token, new SocksWebProxy(new ProxyConfig(
                            IPAddress.Parse("127.0.0.1"),
                            GetNextFreePort(),
                            hostAddressesSocks5.First(),
                            providerSettings.ProxyPort,
                            ProxyConfig.SocksVersion.Five
                        )));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return telegramBotClient;
        }

        public async Task SendAsync(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot, string endpointOptionsString, string text)
        {
            var providerSettings = configurationRoot.GetSection(GetType().Name).Get<ProviderSettings>();
            var telegramBotClient = CreateTelegramBotClient(providerSettings);
            if (text.Length > MaxTelegramStringLength)
            {
                text = text.Substring(0, MaxTelegramStringLength);
            }
            await telegramBotClient.SendTextMessageAsync(new ChatId(endpointOptionsString), text);
        }
    }
}
