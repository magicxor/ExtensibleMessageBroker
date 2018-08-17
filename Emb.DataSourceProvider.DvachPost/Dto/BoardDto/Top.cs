using Newtonsoft.Json;

namespace Emb.DataSourceProvider.DvachPost.Dto.BoardDto
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