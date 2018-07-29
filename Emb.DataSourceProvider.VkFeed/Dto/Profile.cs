using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class Profile
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("sex")]
        public long Sex { get; set; }

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("photo_50")]
        public string Photo50 { get; set; }

        [JsonProperty("photo_100")]
        public string Photo100 { get; set; }

        [JsonProperty("online")]
        public long Online { get; set; }

        [JsonProperty("online_app", NullValueHandling = NullValueHandling.Ignore)]
        public string OnlineApp { get; set; }

        [JsonProperty("online_mobile", NullValueHandling = NullValueHandling.Ignore)]
        public long? OnlineMobile { get; set; }
    }
}