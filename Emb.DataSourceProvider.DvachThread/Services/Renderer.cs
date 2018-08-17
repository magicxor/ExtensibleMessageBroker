using System;
using System.Collections.Generic;
using System.Linq;
using Emb.DataSourceProvider.DvachThread.Dto;
using Emb.DataSourceProvider.DvachThread.Models;

namespace Emb.DataSourceProvider.DvachThread.Services
{
    public class Renderer
    {
        public List<string> RenderAsPlainText(IEnumerable<Thread> threads, Uri siteUri, EndpointOptions endpointOptions)
        {
            var resultItems = threads
                .Select(t =>
                    new UriBuilder(siteUri) { Path = $"{endpointOptions.BoardId}/res/{t.Num}.html" }.Uri
                    + Environment.NewLine
                    + t.Comment)
                .ToList();
            return resultItems;
        }
    }
}
