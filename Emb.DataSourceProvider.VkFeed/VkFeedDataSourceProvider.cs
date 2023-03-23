using System;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emb.Common.Abstractions;
using Emb.Common.Models;
using Emb.Common.Utils;
using Emb.DataSourceProvider.VkFeed.Abstractions;
using Emb.DataSourceProvider.VkFeed.Models;
using Emb.DataSourceProvider.VkFeed.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;

namespace Emb.DataSourceProvider.VkFeed
{
    [Export(typeof(IDataSourceProvider))]
    public class VkFeedDataSourceProvider : IDataSourceProvider
    {
        private readonly Renderer _renderer = new Renderer();
        private readonly DataExtractor _dataExtractor = new DataExtractor();

        public async Task<IDataFetchResult> GetNewItemsAsPlainTextAsync(ILoggerFactory loggerFactory, 
            IConfigurationRoot configurationRoot, 
            string endpointOptionsString, 
            string stateString,
            CancellationToken cancellationToken)
        {
            var logger = loggerFactory.CreateLogger<VkFeedDataSourceProvider>();
            var providerSettings = configurationRoot.GetSection(GetType().Name).Get<ProviderSettings>();
            var endpointOptions = JsonConvert.DeserializeObject<EndpointOptions>(endpointOptionsString);
            var state = JsonConvert.DeserializeObject<State>(stateString) ?? new State();

            var siteUri = new Uri("https://api.vk.com");
            var api = RestService.For<IVkApi>(siteUri.ToString(), new RefitSettings {
                ContentSerializer = new NewtonsoftJsonContentSerializer(),
            });
            var extractedItems = await _dataExtractor.ExtractAsync(logger, api, providerSettings, state, endpointOptions, cancellationToken);
            var filteredItems = _dataExtractor.Filter(extractedItems, state, endpointOptions);
            var renderedItems = _renderer.RenderAsPlainText(filteredItems);

            var lastItem = extractedItems.ResponseItems.LastOrDefault();
            if (lastItem != null)
            {
                state.LastRecordCreatedUtc = DateTimeUtils.TimestampToUtcDateTime(lastItem.Date);
            }
            var result = new DataFetchResult
            {
                Items = renderedItems,
                State = JsonConvert.SerializeObject(state),
            };
            return result;
        }

        public Type GetEndpointOptionsType()
        {
            return typeof(EndpointOptions);
        }

        public Type GetProviderSettingsType()
        {
            return typeof(ProviderSettings);
        }
    }
}
