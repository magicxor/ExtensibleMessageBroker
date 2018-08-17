using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Emb.Common.Models;
using Emb.Common.Utils;
using Emb.DataSourceProvider.DvachThread.Dto;
using Emb.DataSourceProvider.DvachThread.Models;

namespace Emb.DataSourceProvider.DvachThread.Services
{
    public class DataExtractor
    {
        private string StripHtml(string input)
        {
            var dom = new HtmlParser().Parse(input);
            return dom.DocumentElement.ToHtml(new PlainTextMarkupFormatter());
        }

        public List<Thread> Extract(DvachBoard dvachBoard)
        {
            var threads = dvachBoard.Threads
                .OrderBy(t => t.Timestamp)
                .ToList();
            threads.ForEach(t => t.Comment = StripHtml(t.Comment));
            return threads;
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
