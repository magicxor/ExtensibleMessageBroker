namespace Emb.Common.Abstractions
{
    public interface IEndpointInfo
    {
        string ProviderName { get; set; }
        string EndpointOptions { get; set; }
    }
}
