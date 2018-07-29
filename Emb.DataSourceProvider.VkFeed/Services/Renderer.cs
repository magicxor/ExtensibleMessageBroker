using System.Collections.Generic;
using System.Linq;
using Emb.Common.Utils;
using Emb.DataSourceProvider.VkFeed.Dto;
using Emb.DataSourceProvider.VkFeed.Extensions;

namespace Emb.DataSourceProvider.VkFeed.Services
{
    public class Renderer
    {
        public List<string> RenderAsPlainText(IEnumerable<ResponseItem> responseItems)
        {
            return responseItems
                .Select(ri =>
                    DateTimeUtils.TimestampToUtcDateTime(ri.Date)
                    + " " + ri.PostUri()
                    + (ri.SignerId.HasValue ? " " + ri.SignerUri() : string.Empty)
                    + " " + ri.Text)
                .ToList();
        }
    }
}
