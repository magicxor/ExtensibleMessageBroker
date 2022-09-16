using System.Collections.Generic;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.DvachThread.Dto
{
    public class DvachBoardDto
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

        [JsonProperty("filter")]
        public string Filter { get; set; }

        [JsonProperty("threads")]
        public List<Thread> Threads { get; set; }
    }
}
