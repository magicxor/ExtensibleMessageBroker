using System.Collections.Generic;
using Emb.DataSourceProvider.VkFeed.Dto;

namespace Emb.DataSourceProvider.VkFeed.Models
{
    public class VkNewsfeedResult
    {
        public List<Profile> Profiles { get; set; }
        public List<ResponseItem> ResponseItems { get; set; }
    }
}
