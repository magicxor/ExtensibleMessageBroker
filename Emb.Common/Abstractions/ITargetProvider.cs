using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Emb.Common.Abstractions
{
    public interface ITargetProvider
    {
        Task SendAsync(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot, string options, string text, CancellationToken cancellationToken);
        Type GetEndpointOptionsType();
        Type GetProviderSettingsType();
    }
}
