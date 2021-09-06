using System;
using System.Composition;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emb.Common.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Emb.TargetProvider.File
{
    [Export(typeof(ITargetProvider))]
    public class FileTargetProvider : ITargetProvider
    {
        private async Task SaveAsync(string filePath, string text)
        {
            var utf16Bytes = Encoding.Unicode.GetBytes(text + Environment.NewLine + Environment.NewLine + "════════════════════" + Environment.NewLine);
            var utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            using (var sourceStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(utf8Bytes, 0, utf8Bytes.Length);
            }
        }

        public async Task SendAsync(ILoggerFactory loggerFactory, 
            IConfigurationRoot configurationRoot, 
            string endpointOptionsString, 
            string text,
            CancellationToken cancellationToken)
        {
            await SaveAsync(endpointOptionsString, text);
        }

        public Type GetEndpointOptionsType()
        {
            return null;
        }

        public Type GetProviderSettingsType()
        {
            return null;
        }
    }
}
