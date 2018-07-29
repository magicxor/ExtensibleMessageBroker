using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class FluffyLikes
    {
        [JsonProperty("user_likes")]
        public long UserLikes { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }
    }
}