using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Emb.Common.Abstractions;
using Emb.Core.Models;
using Microsoft.Extensions.Logging;

namespace Emb.Core.Services
{
    public class PluginManager: IPluginManager
    {
        private readonly ILogger _logger;

        [ImportMany]
        private IEnumerable<IDataSourceProvider> DataSourceProviders { get; set; }

        [ImportMany]
        private IEnumerable<ITargetProvider> TargetProviders { get; set; }

        public PluginManager(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PluginManager>();
        }

        public PluginSet LoadPlugins()
        {
            try
            {
                var executableLocation = Assembly.GetEntryAssembly().Location;
                _logger.LogDebug($"executable location: {executableLocation}");

                var pluginRootDirectory = Path.GetDirectoryName(executableLocation) ?? throw new InvalidOperationException();
                _logger.LogDebug($"plugin root directory: {executableLocation}");

                var assemblies = Directory
                    .GetFiles(pluginRootDirectory, "*.dll", SearchOption.AllDirectories)
                    .Where(f => f.Contains("Emb.TargetProvider") || f.Contains("Emb.DataSourceProvider"))
                    .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                    .ToList();
                _logger.LogDebug("found assemblies: " + string.Join(", ", assemblies.Select(assembly => assembly.FullName)));

                var configuration = new ContainerConfiguration()
                    .WithAssemblies(assemblies);
                using (var container = configuration.CreateContainer())
                {
                    DataSourceProviders = container.GetExports<IDataSourceProvider>();
                    TargetProviders = container.GetExports<ITargetProvider>();
                }

                var result = new PluginSet()
                {
                    DataSourceProviders = DataSourceProviders.ToList(),
                    TargetProviders = TargetProviders.ToList(),
                };

                _logger.LogDebug("found data source providers: " + string.Join(", ", result.DataSourceProviders.Select(dataSourceProvider => dataSourceProvider.GetType().Name)));
                _logger.LogDebug("found targets: " + string.Join(", ", result.TargetProviders.Select(dataSourceProvider => dataSourceProvider.GetType().Name)));

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "can't load plugins");
                return new PluginSet();
            }
        }
    }
}
