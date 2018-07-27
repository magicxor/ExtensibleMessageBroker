using System.Collections.Generic;

namespace Emb.Common.Abstractions
{
    public interface IDataFetchResult
    {
        IList<string> Items { get; set; }
        string State { get; set; }
    }
}
