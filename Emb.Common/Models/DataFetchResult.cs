using System.Collections.Generic;
using Emb.Common.Abstractions;

namespace Emb.Common.Models
{
    public class DataFetchResult: IDataFetchResult
    {
        public IList<string> Items { get; set; }
        public string State { get; set; }
    }
}
