using System.Web;
using Emb.DataSourceProvider.DvachThread.Dto;

namespace Emb.DataSourceProvider.DvachThread.Extensions
{
    public static class ThreadExtensions
    {
        public static string SubjectDecoded(this Thread thread)
        {
            return string.IsNullOrEmpty(thread.Subject)
                ? string.Empty
                : HttpUtility.HtmlDecode(thread.Subject);
        }
    }
}
