using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Emb.DataSourceProvider.TelegramChannel.Dto;
using Emb.DataSourceProvider.TelegramChannel.Enums;
using Emb.DataSourceProvider.TelegramChannel.Models;
using Microsoft.Extensions.Logging;
using MihaZupan;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.TelegramChannel.Services
{
    public class DataExtractor
    {
        private readonly ILogger _logger;
        private readonly PlainTextMarkupFormatter _plainTextMarkupFormatter = new PlainTextMarkupFormatter();
        private readonly HtmlParser _htmlParser = new HtmlParser();

        public DataExtractor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DataExtractor>();
        }

        private IEnumerable<Post> ParseChannelHtml(ProviderSettings providerSettings, string html)
        {
            var dom = _htmlParser.Parse(html);
            var messageDiv = dom.QuerySelectorAll("div.tgme_widget_message");
            return messageDiv
                .Select(div => new
                {
                    PostLink = div.GetAttribute("data-post") ?? throw new Exception($"Telegram post link not found in: {div.OuterHtml}"),
                    Div = div,
                })
                .Select(x => new Post
                {
                    Id = int.Parse(x.PostLink.Split('/')?.LastOrDefault() ?? throw new Exception($"Telegram post id not found in: {x.Div.OuterHtml}")),
                    Link = $"https://{providerSettings.Hostname}/{x.PostLink}",
                    Text = x.Div.QuerySelector("div.tgme_widget_message_text")?.ToHtml(_plainTextMarkupFormatter)?.Trim(),
                })
                .OrderBy(p => p.Id);
        }

        private IWebProxy GetProxy(ProviderSettings providerSettings)
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
                    if (providerSettings.ProxyAuthentication)
                    {
                        webProxy = new HttpToSocks5Proxy(providerSettings.ProxyHost, providerSettings.ProxyPort, providerSettings.ProxyUsername, providerSettings.ProxyPassword);
                    }
                    else
                    {
                        webProxy = new HttpToSocks5Proxy(providerSettings.ProxyHost, providerSettings.ProxyPort);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return webProxy;
        }

        public async Task<List<Post>> ExtractAsync(ProviderSettings providerSettings, State state, EndpointOptions endpointOptions)
        {
            var proxy = GetProxy(providerSettings);

            using (var httpClientHandler = new HttpClientHandler{AllowAutoRedirect = false, AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip})
            {
                if (proxy != null)
                {
                    httpClientHandler.Proxy = proxy;
                    httpClientHandler.UseProxy = true;
                }
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var uri = $"https://{providerSettings.Hostname}/s/{endpointOptions.ChannelName}";
                    _logger.LogTrace($"fetch {uri}");
                    var response = await httpClient.GetAsync(uri);
                    var content = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Redirect || !response.IsSuccessStatusCode)
                    {
                        throw new Exception($"{uri} request failed: {nameof(response.StatusCode)}={response.StatusCode}, {nameof(content)}={content}");
                    }
                    var posts = ParseChannelHtml(providerSettings, content).ToList();
                    
                    var firstPostId = posts.FirstOrDefault()?.Id;
                    var postEqualityComparer = new PostEqualityComparer();
                    while (firstPostId.HasValue && firstPostId > 0 && firstPostId > state.LastRecordId)
                    {
                        Thread.Sleep(200); // todo: move to config
                        var pageUri = $"https://{providerSettings.Hostname}/s/{endpointOptions.ChannelName}?before={firstPostId}";
                        _logger.LogTrace($"fetch next page: {pageUri}");
                        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, pageUri)
                        {
                            Headers =
                            {
                                {"Pragma", "no-cache"},
                                {"Sec-Fetch-Site", "same-origin"},
                                {"Origin", $"https://{providerSettings.Hostname}"},
                                {"Accept-Encoding", "gzip, deflate"},
                                {"Accept-Language", "en-US,en;q=0.9,ru-RU;q=0.8,ru;q=0.7"},
                                {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.132 Safari/537.36"},
                                {"Sec-Fetch-Mode", "cors"},
                                {"Accept", "application/json, text/javascript, */*; q=0.01"},
                                {"Cache-Control", "no-cache"},
                                {"X-Requested-With", "XMLHttpRequest"},
                                {"Referer", uri}
                            }
                        };
                        var pageResponse = await httpClient.SendAsync(httpRequestMessage);
                        var pageContent = await pageResponse.Content.ReadAsStringAsync();
                        if (pageResponse.StatusCode == HttpStatusCode.Redirect || !pageResponse.IsSuccessStatusCode)
                        {
                            throw new Exception($"{uri} request failed: {nameof(pageResponse.StatusCode)}={pageResponse.StatusCode}, {nameof(pageContent)}={pageContent}");
                        }
                        var pageContentDeserialized = JsonConvert.DeserializeObject<string>(pageContent);
                        var pagePosts = ParseChannelHtml(providerSettings, pageContentDeserialized).Except(posts, postEqualityComparer).ToList();
                        posts.InsertRange(0, pagePosts);
                        firstPostId = pagePosts.FirstOrDefault()?.Id;
                    }
                    
                    _logger.LogInformation($"{posts.Count} posts found");
                    return posts;
                }
            }
        }

        public List<Post> Filter(List<Post> extractedItems, State state, EndpointOptions endpointOptions)
        {
            var filteredItems = extractedItems.AsQueryable();
            var result = filteredItems
                .Where(x => !string.IsNullOrEmpty(x.Text))
                .Where(ri =>
                    ri.Id > state.LastRecordId
                    && (endpointOptions.IncludedPatterns == null || !endpointOptions.IncludedPatterns.Any() || endpointOptions.IncludedPatterns.Any(pattern => Regex.IsMatch(ri.Text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)))
                    && (endpointOptions.ExcludedPatterns == null || !endpointOptions.ExcludedPatterns.Any() || !endpointOptions.ExcludedPatterns.Any(pattern => Regex.IsMatch(ri.Text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))))
                .ToList();

            _logger.LogInformation($"{result.Count} posts match");
            return result;
        }
    }
}
