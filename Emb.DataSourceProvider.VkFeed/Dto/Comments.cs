using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class Comments
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("groups_can_post")]
        public bool GroupsCanPost { get; set; }

        [JsonProperty("can_post")]
        public long CanPost { get; set; }
    }
}