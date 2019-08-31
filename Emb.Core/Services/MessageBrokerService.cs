using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Emb.Common.Utils;
using Emb.Core.Constants;
using Emb.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Schema.Generation;

namespace Emb.Core.Services
{
    public class MessageBrokerService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfigurationRoot _configurationRoot;
        private readonly ILogger _logger;
        private readonly PluginSet _pluginSet;
        private readonly JSchemaGenerator _schemaGenerator;

        public MessageBrokerService(ILoggerFactory loggerFactory, IConfigurationRoot configurationRoot, PluginManager pluginManager, JSchemaGenerator schemaGenerator)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<MessageBrokerService>();
            _configurationRoot = configurationRoot;
            _pluginSet = pluginManager.LoadPlugins();
            _schemaGenerator = schemaGenerator;
        }

        public void GenerateJsonSchema(string basePath, string providerName, string modelName, Type modelType)
        {
            if (modelType != null)
            {
                _logger.LogTrace($"start json schema generation for {providerName}.{modelName} ({nameof(modelType)}: {modelType.Name})");

                var schema = _schemaGenerator.Generate(modelType);
                var schemaFilePath = Path.Combine(basePath, $"{providerName}_{modelName}.schema.json");
                var schemaString = schema.ToString();
                File.WriteAllText(schemaFilePath, schemaString);

                _logger.LogTrace($"schema generation was successful: {schemaFilePath}");
            }
            else
            {
                _logger.LogTrace($"skip json schema generation for {providerName}.{modelName} because {nameof(modelType)} is null");
            }
        }

        public void GenerateJsonSchemas(string basePath)
        {
            _logger.LogInformation($"running {nameof(GenerateJsonSchemas)}...");

            foreach (var dataSourceProvider in _pluginSet.DataSourceProviders)
            {
                var providerName = dataSourceProvider.GetType().Name;

                var endpointOptionsType = dataSourceProvider.GetEndpointOptionsType();
                GenerateJsonSchema(basePath, providerName, Defaults.EndpointOptionsModelName, endpointOptionsType);

                var providerSettingsType = dataSourceProvider.GetProviderSettingsType();
                GenerateJsonSchema(basePath, providerName, Defaults.ProviderSettingsModelName, providerSettingsType);
            }
            foreach (var targetProvider in _pluginSet.TargetProviders)
            {
                var providerName = targetProvider.GetType().Name;

                var endpointOptionsType = targetProvider.GetEndpointOptionsType();
                GenerateJsonSchema(basePath, providerName, Defaults.EndpointOptionsModelName, endpointOptionsType);

                var providerSettingsType = targetProvider.GetProviderSettingsType();
                GenerateJsonSchema(basePath, providerName, Defaults.ProviderSettingsModelName, providerSettingsType);
            }
        }

        public async Task RunOnceAsync(IList<DataFlow> dataFlows)
        {
            _logger.LogInformation($"running {nameof(RunOnceAsync)}...");

            foreach (var dataFlow in dataFlows)
            {
                try
                {
                    var dataSourceProvider = _pluginSet.DataSourceProviders.FirstOrDefault(dsp => dsp.GetType().Name == dataFlow.Source.ProviderName);
                    if (dataSourceProvider == null)
                    {
                        _logger.LogError($"no data source provider found with name {dataFlow.Source.ProviderName}.");
                    }
                    else
                    {
                        var stateString = LoadState(dataFlow.Name);
                        var dataFetchResult = await dataSourceProvider.GetNewItemsAsPlainTextAsync(_loggerFactory, _configurationRoot, dataFlow.Source.EndpointOptions, stateString);
                        SaveState(dataFlow.Name, dataFetchResult.State);
                        foreach (var target in dataFlow.Targets)
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
                                        await targetProvider.SendAsync(_loggerFactory, _configurationRoot, target.EndpointOptions, dataFetchResultItem);
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
                    _logger.LogError(e, $"error during {dataFlow.Name} processing");
                }
            }
        }

        private string GetStateFilePath(string dataFlowName)
        {
            var stateDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? throw new InvalidOperationException();
            return Path.Combine(stateDirectory, "State", FilesystemUtils.CoerceValidFileName(dataFlowName + ".state"));
        }

        private void SaveState(string dataFlowName, string stateString)
        {
            var stateFilePath = GetStateFilePath(dataFlowName);

            _logger.LogDebug($"save state {stateString} of {dataFlowName} to {stateFilePath}");

            new FileInfo(stateFilePath).Directory?.Create();
            File.WriteAllText(stateFilePath, stateString);
        }

        private string LoadState(string dataFlowName)
        {
            var stateFilePath = GetStateFilePath(dataFlowName);

            _logger.LogDebug($"load state of {dataFlowName} from {stateFilePath}");

            return File.Exists(stateFilePath) ? File.ReadAllText(stateFilePath) : string.Empty;
        }
    }
}
