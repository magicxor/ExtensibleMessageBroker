using System;
using System.Composition;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Emb.Common.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Emb.TargetProvider.File
{
    [Export(typeof(ITargetProvider))]
    public class FileTargetProvider : ITargetProvider
    {
        public async Task SendAsync(IConfigurationRoot configurationRoot, string endpointOptionsString, string text)
        {
            var encodedText = Encoding.Unicode.GetBytes(text + Environment.NewLine + Environment.NewLine);
            using (var sourceStream = new FileStream(endpointOptionsString, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            }
        }
    }
}
