using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Emb.Common.Abstractions;
using Emb.Common.Models;
using Emb.DataSourceProvider.Dvach.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;

namespace Emb.DataSourceProvider.Dvach
{
    [Export(typeof(IDataSourceProvider))]
    public class DvachDataSourceProvider : IDataSourceProvider
    {
        private static DateTime TimestampToUtcDateTime(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        }

        public async Task<IDataFetchResult> GetNewItemsAsPlainTextAsync(IConfigurationRoot configurationRoot, string endpointOptionsString, string stateString)
        {
            var providerSettings = configurationRoot.GetSection(GetType().Name).Get<DvachDataSourceProviderSettings>();
            var endpointOptions = endpointOptionsString;
            var state = JsonConvert.DeserializeObject<DvachDataFlowState>(stateString) ?? new DvachDataFlowState();

            var siteUri = new Uri("https://" + providerSettings.Hostname);
            var dvachApi = RestService.For<IDvachApi>(siteUri.ToString());
            var dvachApiResult = await dvachApi.GetBoard(endpointOptions);

            var threads = state.LastRecordCreatedUtc == null 
                ? dvachApiResult.Threads 
                : dvachApiResult.Threads.Where(t => TimestampToUtcDateTime(t.Timestamp) > state.LastRecordCreatedUtc).ToList();

            var resultItems = threads.Select(t => 
                (new UriBuilder(siteUri) { Path = $"{endpointOptions}/res/{t.Num}.html" }).ToString() 
                + Environment.NewLine 
                + t.Subject 
                + Environment.NewLine 
                + t.Comment).ToList();

            if (threads.Any())
            {
                state.LastRecordCreatedUtc = TimestampToUtcDateTime(threads.First().Timestamp);
            }
            
            var result = new DataFetchResult()
            {
                Items = resultItems,
                State = JsonConvert.SerializeObject(state),
            };
            return result;
        }
    }
}
