using Emb.Configurator.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Emb.Configurator.DependencyInjection
{
    public static class ContainerConfiguration
    {
        public static IServiceProvider CreateServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton<MainViewModel>()
                .AddSingleton<MainWindow>()
                .BuildServiceProvider();
        }
    }
}
