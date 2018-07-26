using System.IO;
using System.Reflection;
using System.Threading.Tasks;
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

        private static async Task Main(string[] args)
        {
            var configurationRoot = GetConfigurationRoot();
            var serviceProvider = CreateServiceProvider(configurationRoot);

            var applicationSettings = configurationRoot.GetSection(Defaults.ConfigurationSectionName).Get<ApplicationSettings>();
            var messageBrokerService = serviceProvider.GetService<MessageBrokerService>();
            await messageBrokerService.RunOnceAsync(applicationSettings.DataFlows);
        }
    }
}
