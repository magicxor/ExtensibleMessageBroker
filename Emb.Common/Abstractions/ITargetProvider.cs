using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Emb.Common.Abstractions
{
    public interface ITargetProvider
    {
        Task SendAsync(IConfigurationRoot configurationRoot, string options, string text);
    }
}
