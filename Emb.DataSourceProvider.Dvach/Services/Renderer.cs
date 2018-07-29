using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Emb.DataSourceProvider.Dvach.Formatting;
using Emb.DataSourceProvider.Dvach.Models;

namespace Emb.DataSourceProvider.Dvach.Services
{
    public class Renderer
    {
        public string StripHtml(string input)
        {
            var dom = new HtmlParser().Parse(input);
            return dom.DocumentElement.ToHtml(new PlainTextMarkupFormatter());
        }

        public List<string> RenderAsPlainText(IEnumerable<Thread> threads, Uri siteUri, EndpointOptions endpointOptions)
        {
            var resultItems = threads
                .Select(t =>
                    (new UriBuilder(siteUri) { Path = $"{endpointOptions.BoardId}/res/{t.Num}.html" }).ToString()
                    + Environment.NewLine
                    + t.Subject
                    + Environment.NewLine
                    + StripHtml(t.Comment))
                .ToList();
            return resultItems;
        }
    }
}
