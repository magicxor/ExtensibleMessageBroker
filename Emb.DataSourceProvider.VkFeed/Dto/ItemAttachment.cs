using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class ItemAttachment
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("photo", NullValueHandling = NullValueHandling.Ignore)]
        public PurplePhoto Photo { get; set; }

        [JsonProperty("audio", NullValueHandling = NullValueHandling.Ignore)]
        public PurpleAudio Audio { get; set; }

        [JsonProperty("doc", NullValueHandling = NullValueHandling.Ignore)]
        public PurpleDoc Doc { get; set; }

        [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
        public Link Link { get; set; }

        [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
        public AttachmentVideo Video { get; set; }
    }
}