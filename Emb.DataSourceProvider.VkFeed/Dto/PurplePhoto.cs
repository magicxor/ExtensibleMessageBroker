using System.Collections.Generic;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class PurplePhoto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("album_id")]
        public long AlbumId { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("sizes")]
        public List<ItemSize> Sizes { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("post_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? PostId { get; set; }

        [JsonProperty("access_key")]
        public string AccessKey { get; set; }

        [JsonProperty("lat", NullValueHandling = NullValueHandling.Ignore)]
        public double? Lat { get; set; }

        [JsonProperty("long", NullValueHandling = NullValueHandling.Ignore)]
        public double? Long { get; set; }
    }
}