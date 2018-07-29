using System.Collections.Generic;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class Response
    {
        [JsonProperty("items")]
        public List<ResponseItem> Items { get; set; }

        [JsonProperty("profiles")]
        public List<Profile> Profiles { get; set; }

        [JsonProperty("groups")]
        public List<Group> Groups { get; set; }

        [JsonProperty("next_from")]
        public string NextFrom { get; set; }
    }
}