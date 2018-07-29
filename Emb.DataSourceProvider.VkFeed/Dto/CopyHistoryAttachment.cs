using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class CopyHistoryAttachment
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("doc", NullValueHandling = NullValueHandling.Ignore)]
        public FluffyDoc Doc { get; set; }

        [JsonProperty("audio", NullValueHandling = NullValueHandling.Ignore)]
        public FluffyAudio Audio { get; set; }

        [JsonProperty("photo", NullValueHandling = NullValueHandling.Ignore)]
        public FluffyPhoto Photo { get; set; }
    }
}