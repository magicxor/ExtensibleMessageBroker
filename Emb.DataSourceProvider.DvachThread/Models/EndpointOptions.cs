using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Emb.DataSourceProvider.DvachThread.Models
{
    public class EndpointOptions
    {
        [Required]
        public string BoardId { get; set; }
        public List<string> IncludedPatterns { get; set; } = new List<string>();
        public List<string> ExcludedPatterns { get; set; } = new List<string>();
    }
}
