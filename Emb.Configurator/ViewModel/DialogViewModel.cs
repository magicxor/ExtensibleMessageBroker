using Emb.Configurator.Interfaces;
using GalaSoft.MvvmLight.Command;
using PropertyChanged;

namespace Emb.Configurator.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class DialogViewModel
    {
        public string Title { get; set; }
        public string MessageText { get; set; }
        public string ButtonText { get; set; }

        public RelayCommand<IClosable> CloseCommand { get; } = new RelayCommand<IClosable>(closable => closable.Close());
    }
}
