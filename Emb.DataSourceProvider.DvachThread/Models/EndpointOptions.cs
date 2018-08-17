using System.Collections.Generic;

namespace Emb.DataSourceProvider.DvachThread.Models
{
    public class EndpointOptions
    {
        public string BoardId { get; set; }
        public List<string> IncludedPatterns { get; set; } = new List<string>();
        public List<string> ExcludedPatterns { get; set; } = new List<string>();
    }
}
