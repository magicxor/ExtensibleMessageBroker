using System.Threading.Tasks;
using Emb.DataSourceProvider.VkFeed.Dto;
using Refit;

namespace Emb.DataSourceProvider.VkFeed.Abstractions
{
    public interface IVkApi
    {
        [Get("/method/newsfeed.get?return_banned=0&count=100&filters=post&access_token={accessToken}&v=5.131")]
        Task<VkNewsFeedGetResponse> GetFeedFirstPage(string accessToken);

        [Get("/method/newsfeed.get?return_banned=0&count=100&filters=post&start_from={startFrom}&access_token={accessToken}&v=5.131")]
        Task<VkNewsFeedGetResponse> GetFeedNextPage(string accessToken, string startFrom);
    }
}
