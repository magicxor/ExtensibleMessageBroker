using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
        private readonly ApplicationSettings _applicationSettings;
        private readonly ILogger _logger;
        private readonly PluginSet _pluginSet;
        private readonly JSchemaGenerator _schemaGenerator;
        private const int DefaultTimeoutInSeconds = 900;

        public MessageBrokerService(ILoggerFactory loggerFactory, 
            IConfigurationRoot configurationRoot,
            ApplicationSettings applicationSettings,
            IPluginManager pluginManager, 
            JSchemaGenerator schemaGenerator)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<MessageBrokerService>();
            _configurationRoot = configurationRoot;
            _applicationSettings = applicationSettings;
            _pluginSet = pluginManager.LoadPlugins();
            _schemaGenerator = schemaGenerator;
        }

        private void GenerateJsonSchema(string basePath, string providerName, string modelName, Type modelType)
        {
            if (modelType != null)
            {
                _logger.LogTrace("start json schema generation for {ProviderName}.{ModelName} ({ModelTypeName}: {ModelType})", providerName, modelName, nameof(modelType), modelType.Name);

                var schema = _schemaGenerator.Generate(modelType);
                var schemaFilePath = Path.Combine(basePath, $"{providerName}_{modelName}.schema.json");
                var schemaString = schema.ToString();
                File.WriteAllText(schemaFilePath, schemaString);

                _logger.LogTrace("schema generation was successful: {SchemaFilePath}", schemaFilePath);
            }
            else
            {
                _logger.LogTrace("skip json schema generation for {ProviderName}.{ModelName} because {ModelTypeName} is null", providerName, modelName, nameof(modelType));
            }
        }

        public void GenerateJsonSchemas(string basePath)
        {
            _logger.LogInformation("running {GenerateJsonSchemasName}...", nameof(GenerateJsonSchemas));

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

        public async Task RunOnceAsync(IList<DataFlow> dataFlows, CancellationToken cancellationToken)
        {
            _logger.LogInformation("running {RunOnceAsyncName}...", nameof(RunOnceAsync));

            var sourceTimeout = (_applicationSettings.SourceTimeoutInSeconds.HasValue && _applicationSettings.SourceTimeoutInSeconds.Value > 0)
                ? TimeSpan.FromSeconds(_applicationSettings.SourceTimeoutInSeconds.Value)
                : TimeSpan.FromSeconds(DefaultTimeoutInSeconds);

            var targetTimeout = (_applicationSettings.TargetTimeoutInSeconds.HasValue && _applicationSettings.TargetTimeoutInSeconds.Value > 0)
                ? TimeSpan.FromSeconds(_applicationSettings.TargetTimeoutInSeconds.Value)
                : TimeSpan.FromSeconds(DefaultTimeoutInSeconds);

            foreach (var dataFlow in dataFlows)
            {
                try
                {
                    var dataSourceProvider = _pluginSet.DataSourceProviders.FirstOrDefault(dsp => dsp.GetType().Name == dataFlow.Source.ProviderName);
                    if (dataSourceProvider == null)
                    {
                        _logger.LogError("no data source provider found with name {SourceProviderName}", dataFlow.Source.ProviderName);
                    }
                    else
                    {
                        var stateString = LoadState(dataFlow.Name);
                        var dataFetchResult = await TaskUtils.CancelAfterAsync(
                            ct => dataSourceProvider.GetNewItemsAsPlainTextAsync(_loggerFactory, _configurationRoot, dataFlow.Source.EndpointOptions, stateString, ct),
                            sourceTimeout,
                            cancellationToken);
                        SaveState(dataFlow.Name, dataFetchResult.State);
                        foreach (var target in dataFlow.Targets)
                        {
                            var targetProvider = _pluginSet.TargetProviders.FirstOrDefault(tgp => tgp.GetType().Name == target.ProviderName);
                            if (targetProvider == null)
                            {
                                _logger.LogError("no target provider found with name {TargetProviderName}", target.ProviderName);
                            }
                            else
                            {
                                foreach (var text in dataFetchResult.Items)
                                {
                                    try
                                    {
                                        await TaskUtils.CancelAfterAsync(
                                            ct => targetProvider.SendAsync(_loggerFactory, _configurationRoot, target.EndpointOptions, text, ct),
                                            targetTimeout,
                                            cancellationToken);
                                    }
                                    catch (Exception e)
                                    {
                                        _logger.LogError(e, "error during sending content {Text} via target provider {Name}", text, targetProvider.GetType().Name);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "error during {DataFlowName} processing", dataFlow.Name);
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

            _logger.LogDebug("save state {StateString} of {DataFlowName} to {StateFilePath}", stateString, dataFlowName, stateFilePath);

            new FileInfo(stateFilePath).Directory?.Create();
            File.WriteAllText(stateFilePath, stateString);
        }

        private string LoadState(string dataFlowName)
        {
            var stateFilePath = GetStateFilePath(dataFlowName);

            _logger.LogDebug("load state of {DataFlowName} from {StateFilePath}", dataFlowName, stateFilePath);

            return File.Exists(stateFilePath) ? File.ReadAllText(stateFilePath) : string.Empty;
        }
    }
}
