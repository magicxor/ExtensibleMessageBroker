using System.Windows;
using Emb.Configurator.ViewModel;

namespace Emb.Configurator.Services
{
    public class WindowService
    {
        public void ShowDialogWindow(Window owner, DialogViewModel viewModel)
        {
            var dialogWindow = new DialogWindow(owner, viewModel);
            dialogWindow.ShowDialog();
        }
    }
}
