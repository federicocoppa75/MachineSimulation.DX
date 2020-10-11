using System.Collections.ObjectModel;
using MachineElements.ViewModels.Interfaces;

namespace MaterialRemoval.ViewModels.Base
{
    public abstract class PanelViewModel : PanelElementViewModel
    {
        private ObservableCollection<IMachineElementViewModel> _children = new ObservableCollection<IMachineElementViewModel>();

        public override ObservableCollection<IMachineElementViewModel> Children => _children;
    }
}
