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
            var imageUri = endpointOptions.AddImageHtml.HasValue && endpointOptions.AddImageHtml == true && post.Files != null && post.Files.Any()
                ? new UriBuilder(siteUri) { Path = post.Files.First().Path }.Uri + Environment.NewLine
                : string.Empty;
            
            var postUri = new UriBuilder(siteUri) { Path = $"{endpointOptions.BoardId}/res/{post.Parent}.html", Fragment = post.Num.ToString() }.Uri + Environment.NewLine;
            
            var postSubject = !string.IsNullOrWhiteSpace(post.Subject) && !post.Comment.StartsWith(post.Subject)
                ? $"[{post.Subject}]" + Environment.NewLine
                : string.Empty;
            
            return imageUri
                + postUri
                + postSubject
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
