using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Emb.Common.Models;
using Emb.Common.Utils;
using Emb.DataSourceProvider.VkFeed.Abstractions;
using Emb.DataSourceProvider.VkFeed.Dto;
using Emb.DataSourceProvider.VkFeed.Extensions;
using Emb.DataSourceProvider.VkFeed.Models;
using Microsoft.Extensions.Logging;

namespace Emb.DataSourceProvider.VkFeed.Services
{
    public class DataExtractor
    {
        private async Task<VkNewsFeedGetResponse> GetVkFeedAsync(IVkApi api, string accessToken, string startFrom = null)
        {
            Thread.Sleep(200);
            if (string.IsNullOrWhiteSpace(startFrom))
            {
                return await api.GetFeedFirstPage(accessToken);
            }
            else
            {
                return await api.GetFeedNextPage(accessToken, startFrom);
            }
        }

        public async Task<List<ResponseItem>> ExtractAsync(ILogger logger, IVkApi api, ProviderSettings providerSettings, State state, EndpointOptions endpointOptions)
        {
            VkNewsFeedGetResponse apiResult;
            string startFrom = null;
            var extractedItems = new List<ResponseItem>();
            do
            {
                apiResult = await GetVkFeedAsync(api, providerSettings.AccessToken, startFrom);
                logger.LogDebug($"{nameof(GetVkFeedAsync)} result: {apiResult.Response.Items.Count} items, NextFrom = {apiResult.Response.NextFrom}");
                var nextStartFrom = apiResult.Response.NextFrom;

                // workaround:
                if (string.IsNullOrWhiteSpace(nextStartFrom))
                {
                    apiResult = await GetVkFeedAsync(api, providerSettings.AccessToken, startFrom);
                    logger.LogDebug($"[workaround: retry] {nameof(GetVkFeedAsync)} result: {apiResult.Response.Items.Count} items, NextFrom = {apiResult.Response.NextFrom}");
                }
                startFrom = apiResult.Response.NextFrom;
                // end of workaround

                extractedItems.AddRange(apiResult.Response.Items);
            } while (!string.IsNullOrWhiteSpace(startFrom)
                     && (state.LastRecordCreatedUtc == null || apiResult.AreAllItemsNewerThan(state.LastRecordCreatedUtc.Value))
                     && (endpointOptions.MaxDaysFromNow == null || endpointOptions.MaxDaysFromNow == 0 || apiResult.AreAllItemsNewerThan(DateTime.UtcNow.AddDays(endpointOptions.MaxDaysFromNow.Value * -1))));

            return extractedItems
                .OrderBy(ri => ri.Date)
                .ToList();
        }

        public List<ResponseItem> Filter(List<ResponseItem> extractedItems, State state, EndpointOptions endpointOptions)
        {
            var filteredItems = extractedItems.AsQueryable();
            if (state.LastRecordCreatedUtc != null)
            {
                filteredItems = filteredItems.Where(ri => DateTimeUtils.TimestampToUtcDateTime(ri.Date) > state.LastRecordCreatedUtc.Value);
            }
            filteredItems = filteredItems
                .Where(ri =>
                    (ri.MarkedAsAds == null || ri.MarkedAsAds == 0)
                    && !string.IsNullOrEmpty(ri.Text)
                    && (endpointOptions.IncludedPatterns == null || !endpointOptions.IncludedPatterns.Any() || endpointOptions.IncludedPatterns.Any(pattern => Regex.IsMatch(ri.Text, pattern)))
                    && (endpointOptions.ExcludedPatterns == null || !endpointOptions.ExcludedPatterns.Any() || !endpointOptions.ExcludedPatterns.Any(pattern => Regex.IsMatch(ri.Text, pattern))));
            return filteredItems
                .ToList();
        }
    }
}
