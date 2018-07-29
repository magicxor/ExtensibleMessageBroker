using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class Action
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}