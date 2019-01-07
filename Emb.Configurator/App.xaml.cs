using Emb.Configurator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Emb.Configurator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var container = ContainerConfiguration.CreateServiceProvider();
            var mainWindow = container.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
