using Newtonsoft.Json;

namespace Emb.DataSourceProvider.DvachPost.Dto.ThreadDto
{
    public class File
    {
        [JsonProperty("displayname")]
        public string Displayname { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("duration_secs")]
        public long DurationSecs { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

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
    }
}