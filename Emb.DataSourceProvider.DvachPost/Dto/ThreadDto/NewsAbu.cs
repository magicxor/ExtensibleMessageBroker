﻿using Newtonsoft.Json;

namespace Emb.DataSourceProvider.DvachPost.Dto.ThreadDto
{
    public class NewsAbu
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("num")]
        public long Num { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("views")]
        public long Views { get; set; }
    }
}