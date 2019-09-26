using System.Web;
using Emb.DataSourceProvider.DvachPost.Dto.ThreadDto;

namespace Emb.DataSourceProvider.DvachPost.Extensions
{
    public static class PostExtensions
    {
        public static string SubjectDecoded(this Post post)
        {
            return string.IsNullOrEmpty(post.Subject)
                ? string.Empty
                : HttpUtility.HtmlDecode(post.Subject);
        }
    }
}