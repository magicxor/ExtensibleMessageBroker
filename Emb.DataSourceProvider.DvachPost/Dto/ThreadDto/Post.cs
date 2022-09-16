using System.Collections.Generic;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.DvachPost.Dto.ThreadDto
{
    public class Post
    {
        [JsonProperty("banned")]
        public long Banned { get; set; }

        [JsonProperty("closed")]
        public long Closed { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("endless")]
        public long Endless { get; set; }

        [JsonProperty("files")]
        public List<File> Files { get; set; }

        [JsonProperty("lasthit")]
        public long Lasthit { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("num")]
        public long Num { get; set; }

        [JsonProperty("number")]
        public long Number { get; set; }

        [JsonProperty("op")]
        public long Op { get; set; }

        [JsonProperty("parent")]
        public long Parent { get; set; }

        [JsonProperty("sticky")]
        public long Sticky { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public string Tags { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("trip")]
        public string Trip { get; set; }

        [JsonProperty("views")]
        public long Views { get; set; }
    }
}