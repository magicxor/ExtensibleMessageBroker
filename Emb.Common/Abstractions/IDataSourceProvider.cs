using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Emb.Common.Abstractions
{
    public interface IDataSourceProvider
    {
        Task<IDataFetchResult> GetNewItemsAsPlainTextAsync(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot, string endpointOptionsString, string stateString);
        Type GetEndpointOptionsType();
        Type GetProviderSettingsType();
    }
}
