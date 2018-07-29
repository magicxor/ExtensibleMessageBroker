using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class FluffyDoc
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("ext")]
        public string Ext { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("preview")]
        public Preview Preview { get; set; }

        [JsonProperty("access_key")]
        public string AccessKey { get; set; }
    }
}