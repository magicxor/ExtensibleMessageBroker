using System;
using System.Composition;
using System.Linq;
using System.Threading;
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
        public async Task<IDataFetchResult> GetNewItemsAsPlainTextAsync(ILoggerFactory loggerFactory, 
            IConfigurationRoot configurationRoot, 
            string endpointOptionsString, 
            string stateString,
            CancellationToken cancellationToken)
        {
            var providerSettings = configurationRoot.GetSection(GetType().Name).Get<ProviderSettings>();
            var endpointOptions = JsonConvert.DeserializeObject<EndpointOptions>(endpointOptionsString);
            var state = JsonConvert.DeserializeObject<State>(stateString) ?? new State();
            var dataExtractor = new DataExtractor(loggerFactory);
            var renderer = new Renderer();

            var siteUri = new Uri("https://" + providerSettings.Hostname);
            var api = RestService.For<IDvachApi>(siteUri.ToString());
            
            var extractedItems = await dataExtractor.ExtractAsync(api, state, endpointOptions, cancellationToken);
            var filteredItems = dataExtractor.Filter(extractedItems, state, endpointOptions);
            var renderedItems = renderer.RenderAsPlainText(filteredItems, siteUri, endpointOptions);

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
