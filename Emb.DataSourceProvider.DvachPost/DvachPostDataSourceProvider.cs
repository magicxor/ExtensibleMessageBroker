using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Emb.Common.Abstractions;
using Emb.Common.Models;
using Emb.Common.Utils;
using Emb.DataSourceProvider.DvachPost.Abstractions;
using Emb.DataSourceProvider.DvachPost.Models;
using Emb.DataSourceProvider.DvachPost.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;

namespace Emb.DataSourceProvider.DvachPost
{
    [Export(typeof(IDataSourceProvider))]
    public class DvachPostDataSourceProvider : IDataSourceProvider
    {
        private readonly Renderer _renderer = new Renderer();
        private readonly DataExtractor _dataExtractor = new DataExtractor();

        public async Task<IDataFetchResult> GetNewItemsAsPlainTextAsync(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot, string endpointOptionsString, string stateString)
        {
            var logger = loggerFactory.CreateLogger<DvachPostDataSourceProvider>();
            var providerSettings = configurationRoot.GetSection(GetType().Name).Get<ProviderSettings>();
            var endpointOptions = JsonConvert.DeserializeObject<EndpointOptions>(endpointOptionsString);
            var state = JsonConvert.DeserializeObject<State>(stateString) ?? new State();

            var siteUri = new Uri("https://" + providerSettings.Hostname);
            var api = RestService.For<IDvachApi>(siteUri.ToString());
            var dvachBoard = await api.GetBoard(endpointOptions.BoardId);
            logger.LogDebug($"{dvachBoard.Threads.Count} threads total in {endpointOptions.BoardId}");

            var extractedItems = await _dataExtractor.ExtractAsync(api, state, endpointOptions);
            var filteredItems = _dataExtractor.Filter(extractedItems, state, endpointOptions);
            var renderedItems = _renderer.RenderAsPlainText(filteredItems, siteUri, endpointOptions);

            var lastItem = extractedItems.LastOrDefault();
            if (lastItem != null)
            {
                state.LastRecordCreatedUtc = DateTimeUtils.TimestampToUtcDateTime(lastItem.Timestamp);
            }

            var result = new DataFetchResult()
            {
                Items = renderedItems,
                State = JsonConvert.SerializeObject(state),
            };
            return result;
        }
    }
}
