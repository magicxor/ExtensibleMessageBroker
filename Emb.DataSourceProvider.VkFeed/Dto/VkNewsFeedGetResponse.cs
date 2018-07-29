using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class VkNewsFeedGetResponse
    {
        [JsonProperty("response")]
        public Response Response { get; set; }
    }
}
