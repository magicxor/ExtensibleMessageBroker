using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class Button
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("action")]
        public Action Action { get; set; }
    }
}