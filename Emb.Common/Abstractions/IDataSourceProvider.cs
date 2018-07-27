using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Emb.Common.Abstractions
{
    public interface IDataSourceProvider
    {
        Task<IDataFetchResult> GetNewItemsAsPlainTextAsync(IConfigurationRoot configurationRoot, string endpointOptionsString, string stateString);
    }
}
