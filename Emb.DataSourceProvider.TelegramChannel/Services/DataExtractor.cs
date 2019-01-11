using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Emb.DataSourceProvider.TelegramChannel.Dto;
using Emb.DataSourceProvider.TelegramChannel.Enums;
using Emb.DataSourceProvider.TelegramChannel.Models;
using Microsoft.Extensions.Logging;
using MihaZupan;

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

        private string GetHref(IElement link)
        {
            var href = link.Attributes.GetNamedItem("href").Value;
            return href;
        }

        private Post ParsePostHtml(int id, string html)
        {
            var dom = _htmlParser.Parse(html);
            var textElement = dom.QuerySelector("div.tgme_widget_message_text");
            var linkElement = dom.QuerySelector("div.tgme_widget_message_link > a");
            var href = GetHref(linkElement);
            var result = new Post()
            {
                Id = id,
                Link = href,
                Text = textElement.ToHtml(_plainTextMarkupFormatter).Trim(),
            };
            return result;
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

            using (var httpClientHandler = new HttpClientHandler())
            {
                if (proxy != null)
                {
                    httpClientHandler.Proxy = proxy;
                }
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    var currentPostId = state.LastRecordId + 1;
                    var notFoundPosts = 0;
                    var posts = new List<Post>();
                    do
                    {
                        var uri = $"https://{providerSettings.Hostname}/{endpointOptions.ChannelName}/{currentPostId}?embed=1";
                        _logger.LogTrace($"Try to get {uri}");

                        var response = await httpClient.GetAsync(uri);
                        var content = await response.Content.ReadAsStringAsync();
                        if (content.Contains("Post not found"))
                        {
                            notFoundPosts++;
                        }
                        else if (content.Contains("tgme_widget_message_error") || !response.IsSuccessStatusCode)
                        {
                            throw new Exception($"{uri} request failed: {nameof(response.StatusCode)}={response.StatusCode}, {nameof(content)}={content}");
                        }
                        else
                        {
                            notFoundPosts = 0;
                            var post = ParsePostHtml(currentPostId, content);
                            posts.Add(post);
                        }
                        currentPostId++;
                    } while (notFoundPosts < providerSettings.MaxNotFoundPostsToStop);

                    _logger.LogInformation($"{posts.Count} posts found");
                    return posts;
                }
            }
        }

        public List<Post> Filter(List<Post> extractedItems, State state, EndpointOptions endpointOptions)
        {
            var filteredItems = extractedItems.AsQueryable();
            var result = filteredItems
                .Where(ri =>
                    (endpointOptions.IncludedPatterns == null || !endpointOptions.IncludedPatterns.Any() || endpointOptions.IncludedPatterns.Any(pattern => Regex.IsMatch(ri.Text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)))
                    && (endpointOptions.ExcludedPatterns == null || !endpointOptions.ExcludedPatterns.Any() || !endpointOptions.ExcludedPatterns.Any(pattern => Regex.IsMatch(ri.Text, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))))
                .ToList();

            _logger.LogInformation($"{result.Count} posts match");
            return result;
        }
    }
}
