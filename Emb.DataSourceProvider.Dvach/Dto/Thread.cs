using System.Collections.Generic;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.Dvach.Models
{
    public class Thread
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

        [JsonProperty("files_count")]
        public long FilesCount { get; set; }

        [JsonProperty("lasthit")]
        public long Lasthit { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("num")]
        public string Num { get; set; }

        [JsonProperty("op")]
        public long Op { get; set; }

        [JsonProperty("parent")]
        public string Parent { get; set; }

        [JsonProperty("posts_count")]
        public long PostsCount { get; set; }

        [JsonProperty("sticky")]
        public long Sticky { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("trip")]
        public string Trip { get; set; }
    }
}