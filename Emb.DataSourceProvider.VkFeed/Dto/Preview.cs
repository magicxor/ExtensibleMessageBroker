using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class Preview
    {
        [JsonProperty("photo")]
        public PreviewPhoto Photo { get; set; }

        [JsonProperty("video")]
        public PreviewVideo Video { get; set; }
    }
}