using Emb.DataSourceProvider.DvachPost.Dto.ThreadDto;
using Emb.DataSourceProvider.DvachPost.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emb.DataSourceProvider.DvachPost.Services
{
    public class Renderer
    {
        private string PostToString(Post post, Uri siteUri, EndpointOptions endpointOptions)
        {
            var imageHtml = endpointOptions.AddImageHtml.HasValue && endpointOptions.AddImageHtml == true && post.Files != null && post.Files.Any()
                ? $@"<a href=""{new UriBuilder(siteUri) { Path = post.Files.First().Path }.Uri}"">🖼️</a> "
                : string.Empty;

            return imageHtml
                   + new UriBuilder(siteUri) { Path = $"{endpointOptions.BoardId}/res/{post.Parent}.html", Fragment = post.Num.ToString() }.Uri
                   + Environment.NewLine
                   + post.Comment;
        }

        public List<string> RenderAsPlainText(IEnumerable<Post> posts, Uri siteUri, EndpointOptions endpointOptions)
        {
            var resultItems = posts
                .Select(p => PostToString(p, siteUri, endpointOptions))
                .ToList();
            return resultItems;
        }
    }
}
