using System.Collections.Generic;

namespace Emb.DataSourceProvider.DvachPost.Models
{
    public class EndpointOptions
    {
        public string BoardId { get; set; }

        public bool? ThreadIsSticky { get; set; }
        public List<string> ThreadSubjectIncludedPatterns { get; set; } = new List<string>();
        public List<string> ThreadSubjectExcludedPatterns { get; set; } = new List<string>();
        public List<string> ThreadCommentIncludedPatterns { get; set; } = new List<string>();
        public List<string> ThreadCommentExcludedPatterns { get; set; } = new List<string>();

        public List<string> PostIncludedPatterns { get; set; } = new List<string>();
        public List<string> PostExcludedPatterns { get; set; } = new List<string>();

    }
}
