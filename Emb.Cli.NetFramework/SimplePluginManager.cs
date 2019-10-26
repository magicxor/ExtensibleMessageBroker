using Emb.Common.Abstractions;
using Emb.Core.Models;
using Emb.Core.Services;
using Emb.DataSourceProvider.DvachPost;
using Emb.DataSourceProvider.DvachThread;
using Emb.DataSourceProvider.TelegramChannel;
using Emb.DataSourceProvider.VkFeed;
using Emb.TargetProvider.File;
using Emb.TargetProvider.Telegram;
using System.Collections.Generic;

namespace Emb.Cli.NetFramework
{
    public class SimplePluginManager : IPluginManager
    {
        public PluginSet LoadPlugins()
        {
            return new PluginSet()
            {
                DataSourceProviders = new List<IDataSourceProvider>()
                {
                    new DvachPostDataSourceProvider(),
                    new DvachThreadDataSourceProvider(),
                    new TelegramChannelDataSourceProvider(),
                    new VkFeedDataSourceProvider(),
                },
                TargetProviders = new List<ITargetProvider>()
                {
                    new FileTargetProvider(),
                    new TelegramTargetProvider(),
                },
            };
        }
    }
}
