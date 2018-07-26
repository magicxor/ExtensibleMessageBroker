using System;
using System.Composition;
using System.Threading.Tasks;
using Emb.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Emb.TargetProvider.File
{
    [Export(typeof(ITargetProvider))]
    public class FileTargetProvider : ITargetProvider
    {
        public Task SendAsync(IConfigurationRoot configurationRoot, string endpointOptions, string text)
        {
            throw new NotImplementedException();
        }
    }
}
