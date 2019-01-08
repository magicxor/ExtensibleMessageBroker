using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Emb.Common.Abstractions
{
    public interface ITargetProvider
    {
        Task SendAsync(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot, string options, string text);
        Type GetEndpointOptionsType();
        Type GetProviderSettingsType();
    }
}
