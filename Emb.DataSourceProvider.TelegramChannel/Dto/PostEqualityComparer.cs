using System.Collections.Generic;

namespace Emb.DataSourceProvider.TelegramChannel.Dto
{
    public class PostEqualityComparer: IEqualityComparer<Post>
    {
        public bool Equals(Post x, Post y)
        {
            return x?.Id == y?.Id;
        }

        public int GetHashCode(Post obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}