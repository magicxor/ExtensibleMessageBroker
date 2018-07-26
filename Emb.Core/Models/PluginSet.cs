using System.Collections.Generic;
using Emb.Common.Abstractions;

namespace Emb.Core.Models
{
    public class PluginSet
    {
        public IList<IDataSourceProvider> DataSourceProviders { get; set; }
        public IList<ITargetProvider> TargetProviders { get; set; }
    }
}
