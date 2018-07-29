using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class Views
    {
        [JsonProperty("count")]
        public long Count { get; set; }
    }
}