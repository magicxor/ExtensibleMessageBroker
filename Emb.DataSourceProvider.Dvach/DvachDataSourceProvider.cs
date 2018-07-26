using System;
using System.Composition;
using System.Threading.Tasks;
using Emb.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Emb.DataSourceProvider.Dvach
{
    [Export(typeof(IDataSourceProvider))]
    public class DvachDataSourceProvider : IDataSourceProvider
    {
        public Task<IDataFetchResult> GetNewItemsAsPlainTextAsync(IConfigurationRoot configurationRoot, string endpointOptions)
        {
            throw new NotImplementedException();
        }
    }
}
