using System.Collections.Generic;

namespace Emb.DataSourceProvider.VkFeed.Models
{
    public class EndpointOptions
    {
        public int? MaxDaysFromNow { get; set; }
        public List<string> IncludedPatterns { get; set; } = new List<string>();
        public List<string> ExcludedPatterns { get; set; } = new List<string>();
    }
}
