using System;
using System.Collections.Generic;
using System.Linq;
using Emb.DataSourceProvider.DvachPost.Dto.ThreadDto;
using Emb.DataSourceProvider.DvachPost.Models;

namespace Emb.DataSourceProvider.DvachPost.Services
{
    public class Renderer
    {
        public List<string> RenderAsPlainText(IEnumerable<Post> posts, Uri siteUri, EndpointOptions endpointOptions)
        {
            var resultItems = posts
                .Select(p =>
                    new UriBuilder(siteUri) { Path = $"{endpointOptions.BoardId}/res/{p.Parent}.html", Fragment = p.Num.ToString() }.Uri
                    + Environment.NewLine
                    + p.Comment)
                .ToList();
            return resultItems;
        }
    }
}
