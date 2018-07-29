using System.Collections.Generic;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class CopyHistory
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("from_id")]
        public long FromId { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("post_type")]
        public string PostType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("attachments")]
        public List<CopyHistoryAttachment> Attachments { get; set; }

        [JsonProperty("post_source")]
        public PostSource PostSource { get; set; }
    }
}