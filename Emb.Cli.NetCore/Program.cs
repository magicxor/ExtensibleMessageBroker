using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using Emb.Cli.NetCore.DependencyInjection;
using Emb.Core.Constants;
using Emb.Core.Models;
using Emb.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Emb.Cli.NetCore
{
    internal static class Program
    {
        // see: https://docs.microsoft.com/en-us/windows/desktop/Debug/system-error-codes
        private const int ErrorSuccess = 0;
        private const int ErrorBadArguments = 160;

        private static IConfigurationRoot GetConfigurationRoot()
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile(Defaults.ConfigurationFileName);
            var configurationRoot = configurationBuilder.Build();
            return configurationRoot;
        }

        private static ServiceProvider CreateServiceProvider(IConfigurationRoot configurationRoot)
        {
            var serviceCollection = new ServiceCollection();
            ContainerConfiguration.ConfigureServices(serviceCollection, configurationRoot);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        private static async Task<int> OnParsedAsync(CommandLineOptions commandLineOptions)
        {
            var configurationRoot = GetConfigurationRoot();
            var serviceProvider = CreateServiceProvider(configurationRoot);
            var applicationSettings = configurationRoot.GetSection(Defaults.ConfigurationSectionName).Get<ApplicationSettings>();
            var messageBrokerService = serviceProvider.GetService<MessageBrokerService>();

            if (commandLineOptions.GenerateSchema)
            {
                string jsonSchemasDirectory;
                if (string.IsNullOrWhiteSpace(commandLineOptions.Directory))
                {
                    jsonSchemasDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                }
                else
                {
                    Directory.CreateDirectory(commandLineOptions.Directory);
                    jsonSchemasDirectory = commandLineOptions.Directory;
                }
                messageBrokerService.GenerateJsonSchemas(jsonSchemasDirectory);                
            }
            else
            {
                await messageBrokerService.RunOnceAsync(applicationSettings.DataFlows);
            }

            return ErrorSuccess;
        }

        private static Task<int> OnNotParsed(IEnumerable<Error> errors)
        {
            return Task.FromResult(ErrorBadArguments);
        }

        private static async Task<int> Main(string[] args)
        {
            var parser = new Parser(settings =>
            {
                settings.CaseSensitive = true;
                settings.IgnoreUnknownArguments = false;
                settings.HelpWriter = Console.Error;
            });
            var exitCode = await parser.ParseArguments<CommandLineOptions>(args)
                .MapResult(OnParsedAsync, OnNotParsed);
            return exitCode;
        }
    }
}
