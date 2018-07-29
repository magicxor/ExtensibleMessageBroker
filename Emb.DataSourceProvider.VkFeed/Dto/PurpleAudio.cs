using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class PurpleAudio
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("is_hq")]
        public bool IsHq { get; set; }

        [JsonProperty("album_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? AlbumId { get; set; }

        [JsonProperty("genre_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? GenreId { get; set; }

        [JsonProperty("lyrics_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? LyricsId { get; set; }

        [JsonProperty("no_search", NullValueHandling = NullValueHandling.Ignore)]
        public long? NoSearch { get; set; }
    }
}