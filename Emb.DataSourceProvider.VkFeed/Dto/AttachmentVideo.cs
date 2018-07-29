using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class AttachmentVideo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("comments")]
        public long Comments { get; set; }

        [JsonProperty("views")]
        public long Views { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("photo_130")]
        public string Photo130 { get; set; }

        [JsonProperty("photo_320")]
        public string Photo320 { get; set; }

        [JsonProperty("photo_800")]
        public string Photo800 { get; set; }

        [JsonProperty("access_key")]
        public string AccessKey { get; set; }

        [JsonProperty("can_add")]
        public long CanAdd { get; set; }
    }
}