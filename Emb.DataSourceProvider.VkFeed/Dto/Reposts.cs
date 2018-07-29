using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class Reposts
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("user_reposted")]
        public long UserReposted { get; set; }
    }
}