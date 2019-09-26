using Emb.DataSourceProvider.DvachPost.Dto.ThreadDto;
using Emb.DataSourceProvider.DvachPost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Emb.DataSourceProvider.DvachPost.Extensions;

namespace Emb.DataSourceProvider.DvachPost.Services
{
    public class Renderer
    {
        private bool CommentStartsWithSubject(string comment, string subject)
        {
            comment = comment?.Trim();
            subject = subject?.Trim();
            if (string.IsNullOrWhiteSpace(comment) || string.IsNullOrWhiteSpace(subject))
            {
                return false;
            }
            
            var commentFirstLineMatch = Regex.Match(comment, "^[^\n\r]+");
            if (!commentFirstLineMatch.Success)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(commentFirstLineMatch.Value))
            {
                return false;
            }

            var commentFirstLine = commentFirstLineMatch.Value;
            if (subject.Length > commentFirstLine.Length)
            {
                subject = subject.Substring(0, commentFirstLine.Length);
            }

            return commentFirstLine.StartsWith(subject);
        }
        
        private string PostToString(Post post, Uri siteUri, EndpointOptions endpointOptions)
        {
            var imageUri = endpointOptions.AddImageHtml.HasValue && endpointOptions.AddImageHtml == true && post.Files != null && post.Files.Any()
                ? new UriBuilder(siteUri) { Path = post.Files.First().Path }.Uri + Environment.NewLine
                : string.Empty;
            
            var postUri = new UriBuilder(siteUri) { Path = $"{endpointOptions.BoardId}/res/{post.Parent}.html", Fragment = post.Num.ToString() }.Uri + Environment.NewLine;
            
            var subjectDecoded = post.SubjectDecoded();
            var postSubject = !string.IsNullOrWhiteSpace(subjectDecoded) && !CommentStartsWithSubject(post.Comment, subjectDecoded)
                ? $"[{subjectDecoded}]" + Environment.NewLine
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
