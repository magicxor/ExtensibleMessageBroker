using System.Collections.Generic;
using Newtonsoft.Json;

namespace Emb.DataSourceProvider.VkFeed.Dto
{
    public class ResponseItem
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("source_id")]
        public long SourceId { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("post_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? PostId { get; set; }

        [JsonProperty("post_type", NullValueHandling = NullValueHandling.Ignore)]
        public string PostType { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("marked_as_ads", NullValueHandling = NullValueHandling.Ignore)]
        public long? MarkedAsAds { get; set; }

        [JsonProperty("attachments", NullValueHandling = NullValueHandling.Ignore)]
        public List<ItemAttachment> Attachments { get; set; }

        [JsonProperty("post_source", NullValueHandling = NullValueHandling.Ignore)]
        public PostSource PostSource { get; set; }

        [JsonProperty("comments", NullValueHandling = NullValueHandling.Ignore)]
        public Comments Comments { get; set; }

        [JsonProperty("likes", NullValueHandling = NullValueHandling.Ignore)]
        public PurpleLikes Likes { get; set; }

        [JsonProperty("reposts", NullValueHandling = NullValueHandling.Ignore)]
        public Reposts Reposts { get; set; }

        [JsonProperty("views", NullValueHandling = NullValueHandling.Ignore)]
        public Views Views { get; set; }

        [JsonProperty("photos", NullValueHandling = NullValueHandling.Ignore)]
        public Photos Photos { get; set; }

        [JsonProperty("copy_history", NullValueHandling = NullValueHandling.Ignore)]
        public List<CopyHistory> CopyHistory { get; set; }

        [JsonProperty("signer_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? SignerId { get; set; }

        [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
        public ItemVideo Video { get; set; }
    }
}