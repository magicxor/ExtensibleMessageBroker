using System.Threading.Tasks;
using Emb.DataSourceProvider.DvachPost.Dto.BoardDto;
using Emb.DataSourceProvider.DvachPost.Dto.ThreadDto;
using Refit;

namespace Emb.DataSourceProvider.DvachPost.Abstractions
{
    public interface IDvachApi
    {
        [Get("/{boardId}/catalog_num.json")]
        Task<DvachBoardDto> GetBoard(string boardId);

        [Get("/{boardId}/res/{threadId}.json")]
        Task<DvachThreadDto> GetThread(string boardId, string threadId);
    }
}
