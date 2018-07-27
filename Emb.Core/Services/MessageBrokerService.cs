using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Emb.Core.Models;
using Emb.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Emb.Core.Services
{
    public class MessageBrokerService
    {
        private readonly IConfigurationRoot _configurationRoot;
        private readonly ILogger _logger;
        private readonly PluginSet _pluginSet;

        public MessageBrokerService(ILoggerFactory loggerFactory, PluginManager pluginManager, IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
            _logger = loggerFactory.CreateLogger<MessageBrokerService>();
            _pluginSet = pluginManager.LoadPlugins();
        }

        public async Task RunOnceAsync(IList<DataFlowInfo> dataFlows)
        {
            foreach (var dataFlowInfo in dataFlows)
            {
                try
                {
                    var dataSourceProvider = _pluginSet.DataSourceProviders.FirstOrDefault(dsp => dsp.GetType().Name == dataFlowInfo.Source.ProviderName);
                    if (dataSourceProvider == null)
                    {
                        _logger.LogError($"no data source provider found with name {dataFlowInfo.Source.ProviderName}.");
                    }
                    else
                    {
                        var state = LoadState(dataFlowInfo.Name);
                        var dataFetchResult = await dataSourceProvider.GetNewItemsAsPlainTextAsync(_configurationRoot, dataFlowInfo.Source.EndpointOptions, state);
                        SaveState(dataFlowInfo.Name, dataFetchResult.State);
                        foreach (var target in dataFlowInfo.Targets)
                        {
                            var targetProvider = _pluginSet.TargetProviders.FirstOrDefault(tgp => tgp.GetType().Name == target.ProviderName);
                            if (targetProvider == null)
                            {
                                _logger.LogError($"no target provider found with name {target.ProviderName}.");
                            }
                            else
                            {
                                foreach (var dataFetchResultItem in dataFetchResult.Items)
                                {
                                    try
                                    {
                                        await targetProvider.SendAsync(_configurationRoot, target.EndpointOptions, dataFetchResultItem);
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.LogError(e, $"error during sending content via target provider {targetProvider.GetType().Name}");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"error during {dataFlowInfo.Name} processing");
                }
            }
        }

        private string GetStateFilePath(string dataFlowName)
        {
            var stateDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? throw new InvalidOperationException();
            return Path.Combine(stateDirectory, "State", FilesystemUtils.CoerceValidFileName(dataFlowName + ".state"));
        }

        private void SaveState(string dataFlowName, string state)
        {
            var stateFilePath = GetStateFilePath(dataFlowName);

            _logger.LogDebug($"save state {state} of {dataFlowName} to {stateFilePath}");

            new FileInfo(stateFilePath).Directory?.Create();
            File.WriteAllText(stateFilePath, state);
        }

        private string LoadState(string dataFlowName)
        {
            var stateFilePath = GetStateFilePath(dataFlowName);

            _logger.LogDebug($"load state of {dataFlowName} from {stateFilePath}");

            return File.Exists(stateFilePath) ? File.ReadAllText(stateFilePath) : string.Empty;
        }
    }
}
