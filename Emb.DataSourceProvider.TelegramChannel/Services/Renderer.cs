using System;
using System.Collections.Generic;
using System.Linq;
using Emb.DataSourceProvider.TelegramChannel.Dto;

namespace Emb.DataSourceProvider.TelegramChannel.Services
{
    public class Renderer
    {
        public List<string> RenderAsPlainText(IEnumerable<Post> posts)
        {
            var resultItems = posts
                .Select(p => p.Link + Environment.NewLine + p.Text)
                .ToList();
            return resultItems;
        }
    }
}
