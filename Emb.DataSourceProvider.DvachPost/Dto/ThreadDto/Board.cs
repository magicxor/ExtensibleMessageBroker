using System.Collections.Generic;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.DvachPost.Dto.ThreadDto
{
    public class Board
    {
        [JsonProperty("bump_limit")]
        public long BumpLimit { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("default_name")]
        public string DefaultName { get; set; }

        [JsonProperty("enable_dices")]
        public bool EnableDices { get; set; }

        [JsonProperty("enable_flags")]
        public bool EnableFlags { get; set; }

        [JsonProperty("enable_icons")]
        public bool EnableIcons { get; set; }

        [JsonProperty("enable_likes")]
        public bool EnableLikes { get; set; }

        [JsonProperty("enable_names")]
        public bool EnableNames { get; set; }

        [JsonProperty("enable_oekaki")]
        public bool EnableOekaki { get; set; }

        [JsonProperty("enable_posting")]
        public bool EnablePosting { get; set; }

        [JsonProperty("enable_sage")]
        public bool EnableSage { get; set; }

        [JsonProperty("enable_shield")]
        public bool EnableShield { get; set; }

        [JsonProperty("enable_subject")]
        public bool EnableSubject { get; set; }

        [JsonProperty("enable_thread_tags")]
        public bool EnableThreadTags { get; set; }

        [JsonProperty("enable_trips")]
        public bool EnableTrips { get; set; }

        [JsonProperty("file_types")]
        public List<string> FileTypes { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("info_outer")]
        public string InfoOuter { get; set; }

        [JsonProperty("max_comment")]
        public long MaxComment { get; set; }

        [JsonProperty("max_files_size")]
        public long MaxFilesSize { get; set; }

        [JsonProperty("max_pages")]
        public long MaxPages { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("threads_per_page")]
        public long ThreadsPerPage { get; set; }
    }
}