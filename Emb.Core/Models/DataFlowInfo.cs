using System.Collections.Generic;
using Emb.Common.Abstractions;

namespace Emb.Core.Models
{
    public class DataFlowInfo
    {
        public string Name { get; set; }
        public EndpointInfo Source { get; set; }
        public IList<EndpointInfo> Targets { get; set; }
    }
}
