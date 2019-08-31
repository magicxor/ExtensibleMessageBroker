using Emb.Common.Abstractions;
using Emb.Common.Models;
using Emb.DataSourceProvider.TelegramChannel.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Emb.DataSourceProvider.TelegramChannel.Services;

namespace Emb.DataSourceProvider.TelegramChannel
{
    [Export(typeof(IDataSourceProvider))]
    public class TelegramChannelDataSourceProvider : IDataSourceProvider
    {
        public async Task<IDataFetchResult> GetNewItemsAsPlainTextAsync(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot, string endpointOptionsString, string stateString)
        {
            var providerSettings = configurationRoot.GetSection(GetType().Name).Get<ProviderSettings>();
            var endpointOptions = JsonConvert.DeserializeObject<EndpointOptions>(endpointOptionsString);
            var state = JsonConvert.DeserializeObject<State>(stateString) ?? new State();
            var dataExtractor = new DataExtractor(loggerFactory);
            var renderer = new Renderer();

            var extractedItems = await dataExtractor.ExtractAsync(providerSettings, state, endpointOptions);
            var filteredItems = dataExtractor.Filter(extractedItems, state, endpointOptions);
            var renderedItems = renderer.RenderAsPlainText(filteredItems);

            var lastItem = extractedItems.LastOrDefault();
            if (lastItem != null)
            {
                state.LastRecordId = lastItem.Id;
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
