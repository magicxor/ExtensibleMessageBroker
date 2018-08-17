using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Emb.Common.Models;
using Emb.Common.Utils;
using Emb.DataSourceProvider.DvachPost.Abstractions;
using Emb.DataSourceProvider.DvachPost.Models;

namespace Emb.DataSourceProvider.DvachPost.Services
{
    public class DataExtractor
    {
        private string StripHtml(string input)
        {
            var dom = new HtmlParser().Parse(input);
            return dom.DocumentElement.ToHtml(new PlainTextMarkupFormatter());
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
                    && (endpointOptions.ThreadSubjectIncludedPatterns == null || !endpointOptions.ThreadSubjectIncludedPatterns.Any() || endpointOptions.ThreadSubjectIncludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase)))
                    && (endpointOptions.ThreadSubjectExcludedPatterns == null || !endpointOptions.ThreadSubjectExcludedPatterns.Any() || !endpointOptions.ThreadSubjectExcludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase)))
                    && (endpointOptions.ThreadCommentIncludedPatterns == null || !endpointOptions.ThreadCommentIncludedPatterns.Any() || endpointOptions.ThreadCommentIncludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase)))
                    && (endpointOptions.ThreadCommentExcludedPatterns == null || !endpointOptions.ThreadCommentExcludedPatterns.Any() || !endpointOptions.ThreadCommentExcludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase))))
                .ToList();
        }

        private async Task<List<Dto.ThreadDto.Post>> ExtractPostsAsync(IDvachApi api, EndpointOptions endpointOptions, IList<string> threadIdentifiers)
        {
            var result = new List<Dto.ThreadDto.Post>();
            foreach (var threadId in threadIdentifiers)
            {
                var threadDto = await api.GetThread(endpointOptions.BoardId, threadId);
                var posts = threadDto?.Threads?.FirstOrDefault()?.Posts ??
                            new List<Dto.ThreadDto.Post>();
                result.AddRange(posts);
            }
            return result.OrderBy(p => p.Timestamp).ToList();
        }

        public async Task<List<Dto.ThreadDto.Post>> ExtractAsync(IDvachApi api, State state, EndpointOptions endpointOptions)
        {
            var dvachBoard = await api.GetBoard(endpointOptions.BoardId);
            var extractedThreads = ExtractThreads(dvachBoard);
            var filteredThreads = FilterThreads(extractedThreads, state, endpointOptions);
            var extractedPosts = await ExtractPostsAsync(api, endpointOptions, filteredThreads.Select(p => p.Num).ToList());
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
                    (endpointOptions.PostIncludedPatterns == null || !endpointOptions.PostIncludedPatterns.Any() || endpointOptions.PostIncludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase)))
                    && (endpointOptions.PostExcludedPatterns == null || !endpointOptions.PostExcludedPatterns.Any() || !endpointOptions.PostExcludedPatterns.Any(pattern => Regex.IsMatch(ri.Comment, pattern, RegexOptions.IgnoreCase))))
                .ToList();
        }
    }
}
