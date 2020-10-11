using GalaSoft.MvvmLight;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Messages.MenuCommands;
using System.Collections.ObjectModel;

namespace MachineElements.ViewModels
{
    public class MachineStructViewModel : ViewModelBase
    {
        public ObservableCollection<IMachineElementViewModel> Machines { get; set; } = new ObservableCollection<IMachineElementViewModel>();

        public MachineStructViewModel()
        {
            MessengerInstance.Register<MachineLoadMessage>(this, OnMachineLoadMessage);
        }

        private void OnMachineLoadMessage(MachineLoadMessage msg)
        {
            Machines.Clear();

            foreach (var item in msg.Machine)
            {
                Machines.Add(item);
            }
        }
    }
}
