using Emb.Core.Models;

namespace Emb.Core.Services
{
    public interface IPluginManager
    {
        PluginSet LoadPlugins();
    }
}
