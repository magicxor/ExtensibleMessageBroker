using System.Collections.Generic;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class ItemVideo
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("items")]
        public List<VideoItem> Items { get; set; }
    }
}