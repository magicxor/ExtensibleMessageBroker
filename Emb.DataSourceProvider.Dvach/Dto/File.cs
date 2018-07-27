using System;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.Dvach.Models
{
    public class File
    {
        [JsonProperty("displayname")]
        public string Displayname { get; set; }

        [JsonProperty("fullname", NullValueHandling = NullValueHandling.Ignore)]
        public string Fullname { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("md5", NullValueHandling = NullValueHandling.Ignore)]
        public string Md5 { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("nsfw", NullValueHandling = NullValueHandling.Ignore)]
        public long? Nsfw { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("tn_height")]
        public long TnHeight { get; set; }

        [JsonProperty("tn_width")]
        public long TnWidth { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Duration { get; set; }

        [JsonProperty("duration_secs", NullValueHandling = NullValueHandling.Ignore)]
        public long? DurationSecs { get; set; }

        [JsonProperty("install", NullValueHandling = NullValueHandling.Ignore)]
        public string Install { get; set; }

        [JsonProperty("pack", NullValueHandling = NullValueHandling.Ignore)]
        public string Pack { get; set; }

        [JsonProperty("sticker", NullValueHandling = NullValueHandling.Ignore)]
        public string Sticker { get; set; }
    }
}