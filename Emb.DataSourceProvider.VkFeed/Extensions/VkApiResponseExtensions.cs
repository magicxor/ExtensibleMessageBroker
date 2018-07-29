using System;
using System.Linq;
using Emb.Common.Utils;
using Emb.DataSourceProvider.VkFeed.Dto;

namespace Emb.DataSourceProvider.VkFeed.Extensions
{
    public static class VkApiResponseExtensions
    {
        public static bool AreAllItemsNewerThan(this VkNewsFeedGetResponse vkNewsFeedGetResponse, DateTime utcDateTime)
        {
            return !vkNewsFeedGetResponse.Response.Items.Any(ri => DateTimeUtils.TimestampToUtcDateTime(ri.Date) <= utcDateTime);
        }

        public static string PostUri(this ResponseItem responseItem)
        {
            return $"https://vk.com/wall{responseItem.SourceId}_{responseItem.PostId}";
        }

        public static string SignerUri(this ResponseItem responseItem)
        {
            return $"https://vk.com/id{responseItem.SignerId}";
        }
    }
}
