using System.Collections.Generic;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.DvachPost.Dto.ThreadDto
{
    public class DvachThreadDto
    {
        [JsonProperty("Board")]
        public string Board { get; set; }

        [JsonProperty("BoardInfo")]
        public string BoardInfo { get; set; }

        [JsonProperty("BoardInfoOuter")]
        public string BoardInfoOuter { get; set; }

        [JsonProperty("BoardName")]
        public string BoardName { get; set; }

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

        [JsonProperty("bump_limit")]
        public long BumpLimit { get; set; }

        [JsonProperty("current_thread")]
        public string CurrentThread { get; set; }

        [JsonProperty("default_name")]
        public string DefaultName { get; set; }

        [JsonProperty("enable_dices")]
        public long EnableDices { get; set; }

        [JsonProperty("enable_flags")]
        public long EnableFlags { get; set; }

        [JsonProperty("enable_icons")]
        public long EnableIcons { get; set; }

        [JsonProperty("enable_images")]
        public long EnableImages { get; set; }

        [JsonProperty("enable_likes")]
        public long EnableLikes { get; set; }

        [JsonProperty("enable_names")]
        public long EnableNames { get; set; }

        [JsonProperty("enable_oekaki")]
        public long EnableOekaki { get; set; }

        [JsonProperty("enable_posting")]
        public long EnablePosting { get; set; }

        [JsonProperty("enable_sage")]
        public long EnableSage { get; set; }

        [JsonProperty("enable_shield")]
        public long EnableShield { get; set; }

        [JsonProperty("enable_subject")]
        public long EnableSubject { get; set; }

        [JsonProperty("enable_thread_tags")]
        public long EnableThreadTags { get; set; }

        [JsonProperty("enable_trips")]
        public long EnableTrips { get; set; }

        [JsonProperty("enable_video")]
        public long EnableVideo { get; set; }

        [JsonProperty("files_count")]
        public long FilesCount { get; set; }

        [JsonProperty("is_board")]
        public long IsBoard { get; set; }

        [JsonProperty("is_closed")]
        public long IsClosed { get; set; }

        [JsonProperty("is_index")]
        public long IsIndex { get; set; }

        [JsonProperty("max_comment")]
        public long MaxComment { get; set; }

        [JsonProperty("max_files_size")]
        public long MaxFilesSize { get; set; }

        [JsonProperty("max_num")]
        public long MaxNum { get; set; }

        [JsonProperty("news_abu")]
        public List<NewsAbu> NewsAbu { get; set; }

        [JsonProperty("posts_count")]
        public long PostsCount { get; set; }

        [JsonProperty("threads")]
        public List<Thread> Threads { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("top")]
        public List<Top> Top { get; set; }

        [JsonProperty("unique_posters")]
        public string UniquePosters { get; set; }
    }
}
