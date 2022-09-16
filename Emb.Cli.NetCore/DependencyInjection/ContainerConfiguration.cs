using System;
using System.IO;
using System.Reflection;
using Emb.Core.Constants;
using Emb.Core.Models;
using Emb.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Schema.Generation;

namespace Emb.Cli.NetCore.DependencyInjection
{
    public static class ContainerConfiguration
    {
        private static ILoggerFactory CreateLoggerFactory(ApplicationSettings applicationSettings)
        {
            var logPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? throw new InvalidOperationException(), applicationSettings.LogDirectoryName, "log-{Date}.txt");
            var loggerFactory = LoggerFactory.Create(builder => builder
                .AddConsole()
                .AddFile(logPath, applicationSettings.LogLevel, retainedFileCountLimit: 10)
            );
            return loggerFactory;
        }

        public static void ConfigureServices(IServiceCollection services, IConfigurationRoot configurationRoot)
        {
            var applicationSettings = configurationRoot.GetSection(Defaults.ConfigurationSectionName).Get<ApplicationSettings>();
            services
                .AddSingleton<IConfigurationRoot>(configurationRoot)
                .AddSingleton<ApplicationSettings>(applicationSettings)
                .AddScoped<ILoggerFactory>(c => CreateLoggerFactory(applicationSettings))
                .AddScoped<JSchemaGenerator>()
                .AddScoped<IPluginManager, PluginManager>()
                .AddScoped<MessageBrokerService>();
        }
    }
}
