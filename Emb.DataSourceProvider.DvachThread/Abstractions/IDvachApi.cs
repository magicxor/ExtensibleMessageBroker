using System.Threading;
using System.Threading.Tasks;
using Emb.DataSourceProvider.DvachThread.Dto;
using Refit;

namespace Emb.DataSourceProvider.DvachThread.Abstractions
{
    public interface IDvachApi
    {
        [Get("/{boardId}/catalog_num.json")]
        Task<DvachBoardDto> GetBoard(string boardId, CancellationToken cancellationToken);
    }
}
