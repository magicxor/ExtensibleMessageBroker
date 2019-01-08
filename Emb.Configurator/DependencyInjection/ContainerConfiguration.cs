using Emb.Configurator.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using Emb.Configurator.Services;

namespace Emb.Configurator.DependencyInjection
{
    public static class ContainerConfiguration
    {
        public static IServiceProvider CreateServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton<WindowService>()
                .AddSingleton<MainViewModel>()
                .AddSingleton<MainWindow>()
                .BuildServiceProvider();
        }
    }
}
