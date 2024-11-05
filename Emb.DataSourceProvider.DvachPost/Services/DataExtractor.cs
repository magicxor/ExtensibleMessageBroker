using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Emb.Common.Utils;
using Emb.DataSourceProvider.DvachPost.Abstractions;
using Emb.DataSourceProvider.DvachPost.Models;
using Microsoft.Extensions.Logging;

namespace Emb.DataSourceProvider.DvachPost.Services
{
    public class DataExtractor
    {
        private readonly ILogger _logger;

        public DataExtractor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DvachPostDataSourceProvider>();
        }

        private string StripHtml(string input)
        {
            var dom = new HtmlParser().Parse(input);
            return dom.DocumentElement?.ToHtml(new PlainTextMarkupFormatter());
        }

        private List<Dto.BoardDto.Thread> ExtractThreads(Dto.BoardDto.DvachBoardDto dvachBoard)
        {
            var threads = dvachBoard.Threads;
            threads.ForEach(t => t.Comment = StripHtml(t.Comment));
            return threads;
        }

        private List<Dto.BoardDto.Thread> FilterThreads(List<Dto.BoardDto.Thread> extractedItems, State state, EndpointOptions endpointOptions)
        {
            return extractedItems
                .Where(ri =>
                    (endpointOptions.ThreadIsSticky == null || (endpointOptions.ThreadIsSticky.Value == false && ri.Sticky == 0) || (endpointOptions.ThreadIsSticky.Value == true && ri.Sticky > 0))
                    && (endpointOptions.ThreadSubjectIncludedPatterns == null || !endpointOptions.ThreadSubjectIncludedPatterns.Any() || endpointOptions.ThreadSubjectIncludedPatterns.Any(pattern => Regex.IsMatch(ri.Subject, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)))
                    && (endpointOptions.ThreadSubjectExcludedPatterns == null || !endpointOptions.ThreadSubjectExcludedPatterns.Any() || !endpointOptions.ThreadSubjectExcludedPatterns.Any(pattern => Regex.IsMatch(ri.Subject, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)))
                    && (endpointOptions.ThreadCommentIncludedPatterns == null || !endpointOptions.ThreadCommentIncludedPatterns.Any() || endpointOptions.ThreadCommentIncludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)))
                    && (endpointOptions.ThreadCommentExcludedPatterns == null || !endpointOptions.ThreadCommentExcludedPatterns.Any() || !endpointOptions.ThreadCommentExcludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))))
                .ToList();
        }

        private async Task<List<Dto.ThreadDto.Post>> ExtractPostsAsync(IDvachApi api, EndpointOptions endpointOptions, IList<long> threadIdentifiers, CancellationToken cancellationToken)
        {
            var result = new List<Dto.ThreadDto.Post>();
            foreach (var threadId in threadIdentifiers)
            {
                var threadDto = await api.GetThread(endpointOptions.BoardId, threadId.ToString(), cancellationToken);
                var posts = threadDto?.Threads?.FirstOrDefault()?.Posts ??
                            new List<Dto.ThreadDto.Post>();
                result.AddRange(posts);
            }
            result.ForEach(p => p.Comment = StripHtml(p.Comment));
            return result.OrderBy(p => p.Timestamp).ToList();
        }

        public async Task<List<Dto.ThreadDto.Post>> ExtractAsync(IDvachApi api, State state, EndpointOptions endpointOptions, CancellationToken cancellationToken)
        {
            var dvachBoard = await api.GetBoard(endpointOptions.BoardId, cancellationToken);

            var extractedThreads = ExtractThreads(dvachBoard);
            _logger.LogInformation("{ExtractedThreadsCount} threads total in {EndpointOptionsBoardId}", extractedThreads.Count, endpointOptions.BoardId);

            var filteredThreads = FilterThreads(extractedThreads, state, endpointOptions);
            _logger.LogInformation("{FilteredThreadsCount} relevant threads in {EndpointOptionsBoardId}", filteredThreads.Count, endpointOptions.BoardId);

            var extractedPosts = await ExtractPostsAsync(api, endpointOptions, filteredThreads.Select(p => p.Num).ToList(), cancellationToken);
            _logger.LogInformation("{ExtractedPostsCount} posts extracted from {EndpointOptionsBoardId} threads", extractedPosts.Count, endpointOptions.BoardId);

            return extractedPosts;
        }

        public List<Dto.ThreadDto.Post> Filter(List<Dto.ThreadDto.Post> extractedItems, State state, EndpointOptions endpointOptions)
        {
            var filteredItems = extractedItems.AsQueryable();
            if (state.LastRecordCreatedUtc != null)
            {
                filteredItems = filteredItems.Where(t => DateTimeUtils.TimestampToUtcDateTime(t.Timestamp) > state.LastRecordCreatedUtc);
            }
            return filteredItems
                .Where(ri =>
                    (endpointOptions.PostIncludedPatterns == null || !endpointOptions.PostIncludedPatterns.Any() || endpointOptions.PostIncludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)))
                    && (endpointOptions.PostExcludedPatterns == null || !endpointOptions.PostExcludedPatterns.Any() || !endpointOptions.PostExcludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline))))
                .ToList();
        }
    }
}
