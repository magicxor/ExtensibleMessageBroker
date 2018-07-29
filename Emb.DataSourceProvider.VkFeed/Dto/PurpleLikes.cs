using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class PurpleLikes
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("user_likes")]
        public long UserLikes { get; set; }

        [JsonProperty("can_like")]
        public long CanLike { get; set; }

        [JsonProperty("can_publish")]
        public long CanPublish { get; set; }
    }
}