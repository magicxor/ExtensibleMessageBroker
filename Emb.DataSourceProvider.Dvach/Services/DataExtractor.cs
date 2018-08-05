using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Emb.Common.Models;
using Emb.Common.Utils;
using Emb.DataSourceProvider.Dvach.Models;

namespace Emb.DataSourceProvider.Dvach.Services
{
    public class DataExtractor
    {
        public List<Thread> Extract(DvachBoard dvachBoard)
        {
            return dvachBoard.Threads
                .OrderBy(t => t.Timestamp)
                .ToList();
        }

        public List<Thread> Filter(List<Thread> extractedItems, State state, EndpointOptions endpointOptions)
        {
            var filteredItems = extractedItems.AsQueryable();
            if (state.LastRecordCreatedUtc != null)
            {
                filteredItems = filteredItems.Where(t => DateTimeUtils.TimestampToUtcDateTime(t.Timestamp) > state.LastRecordCreatedUtc);
            }
            return filteredItems
                .Where(ri =>
                    (endpointOptions.IncludedPatterns == null || !endpointOptions.IncludedPatterns.Any() || endpointOptions.IncludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase)))
                    && (endpointOptions.ExcludedPatterns == null || !endpointOptions.ExcludedPatterns.Any() || !endpointOptions.ExcludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase))))
                .ToList();
        }
    }
}
