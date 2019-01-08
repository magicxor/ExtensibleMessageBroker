using Emb.Configurator.Interfaces;
using Emb.Configurator.ViewModel;
using System.Windows;

namespace Emb.Configurator
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window, IClosable
    {
        private readonly DialogViewModel _viewModel;

        public DialogWindow(Window owner, DialogViewModel viewModel)
        {
            this.Owner = owner;
            _viewModel = viewModel;
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
