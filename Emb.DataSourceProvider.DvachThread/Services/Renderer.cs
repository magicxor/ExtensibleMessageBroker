using System;
using System.Collections.Generic;
using System.Linq;
using Emb.DataSourceProvider.DvachThread.Dto;
using Emb.DataSourceProvider.DvachThread.Models;

namespace Emb.DataSourceProvider.DvachThread.Services
{
    public class Renderer
    {
        private string ThreadToString(Thread thread, Uri siteUri, EndpointOptions endpointOptions)
        {
            var imageUri = endpointOptions.AddImageHtml.HasValue && endpointOptions.AddImageHtml == true && thread.Files != null && thread.Files.Any()
                ? new UriBuilder(siteUri) { Path = thread.Files.First().Path }.Uri + Environment.NewLine
                : string.Empty;
            
            var threadUri = new UriBuilder(siteUri) { Path = $"{endpointOptions.BoardId}/res/{thread.Num}.html" }.Uri + Environment.NewLine;
            
            var threadSubject = !string.IsNullOrWhiteSpace(thread.Subject) && !thread.Comment.StartsWith(thread.Subject)
                ? $"[{thread.Subject}]" + Environment.NewLine
                : string.Empty;
            
            return imageUri
                + threadUri
                + threadSubject
                + thread.Comment;
        }

        public List<string> RenderAsPlainText(IEnumerable<Thread> threads, Uri siteUri, EndpointOptions endpointOptions)
        {
            var resultItems = threads
                .Select(p => ThreadToString(p, siteUri, endpointOptions))
                .ToList();
            return resultItems;
        }
    }
}
