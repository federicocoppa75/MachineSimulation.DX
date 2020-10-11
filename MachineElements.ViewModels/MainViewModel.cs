using GalaSoft.MvvmLight;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Messages.MenuCommands;
using System.Collections.ObjectModel;
using System.Linq;

namespace MachineElements.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<IMachineElementViewModel> Machines { get; set; } = new ObservableCollection<IMachineElementViewModel>();

        protected void NotifyMachineChanged()
        {
            MessengerInstance.Send(new MachineLoadMessage()
            {
                Machine = Machines.ToList()
            });
        }
    }
}
