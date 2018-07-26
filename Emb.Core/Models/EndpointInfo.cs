using Emb.Common.Abstractions;

namespace Emb.Core.Models
{
    public class EndpointInfo: IEndpointInfo
    {
        public string ProviderName { get; set; }
        public string EndpointOptions { get; set; }
    }
}
