using System.Threading.Tasks;
using Emb.DataSourceProvider.Dvach.Models;
using Refit;

namespace Emb.DataSourceProvider.Dvach.Abstractions
{
    public interface IDvachApi
    {
        [Get("/{boardId}/catalog_num.json")]
        Task<DvachBoard> GetBoard(string boardId);
    }
}
