﻿using Newtonsoft.Json;

namespace Emb.DataSourceProvider.DvachThread.Dto
{
    public class Top
    {
        [JsonProperty("board")]
        public string Board { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}