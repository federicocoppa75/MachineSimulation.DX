using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MachineElements.ViewModels.Interfaces;

namespace MaterialRemoval.ViewModels.Base
{
    public abstract class PanelSectionViewModel : PanelElementViewModel
    {
        // gli elementi "PanelSection" non hanno figli: chiudono l'albero
        private static ObservableCollection<IMachineElementViewModel> _children;

        public override ObservableCollection<IMachineElementViewModel> Children => _children;

        static PanelSectionViewModel()
        {
            _children = new ObservableCollection<IMachineElementViewModel>();
        }
    }
}
