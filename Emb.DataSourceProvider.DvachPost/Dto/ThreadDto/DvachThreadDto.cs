namespace Emb.DataSourceProvider.DvachPost.Dto.ThreadDto
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class DvachThreadDto
    {
        [JsonProperty("advert_bottom_image")]
        public string AdvertBottomImage { get; set; }

        [JsonProperty("advert_bottom_link")]
        public string AdvertBottomLink { get; set; }

        [JsonProperty("advert_mobile_image")]
        public string AdvertMobileImage { get; set; }

        [JsonProperty("advert_mobile_link")]
        public string AdvertMobileLink { get; set; }

        [JsonProperty("advert_top_image")]
        public string AdvertTopImage { get; set; }

        [JsonProperty("advert_top_link")]
        public string AdvertTopLink { get; set; }

        [JsonProperty("board_banner_image")]
        public string BoardBannerImage { get; set; }

        [JsonProperty("board_banner_link")]
        public string BoardBannerLink { get; set; }

        [JsonProperty("current_thread")]
        public long CurrentThread { get; set; }

        [JsonProperty("files_count")]
        public long FilesCount { get; set; }

        [JsonProperty("is_board")]
        public bool IsBoard { get; set; }

        [JsonProperty("is_closed")]
        public long IsClosed { get; set; }

        [JsonProperty("is_index")]
        public bool IsIndex { get; set; }

        [JsonProperty("max_num")]
        public long MaxNum { get; set; }

        [JsonProperty("posts_count")]
        public long PostsCount { get; set; }

        [JsonProperty("thread_first_image")]
        public string ThreadFirstImage { get; set; }

        [JsonProperty("threads")]
        public List<Thread> Threads { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("unique_posters")]
        public long UniquePosters { get; set; }
    }
}
