using GalaSoft.MvvmLight;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Interfaces.Links;
using MachineElements.ViewModels.Messages.Links;
using MachineElements.ViewModels.Messages.MenuCommands;
using System.Collections.ObjectModel;
using System.Linq;

namespace MachineElements.ViewModels.Links
{
    public class LinksViewModel : ViewModelBase
    {
        public ObservableCollection<ILinkViewModel> Links { get; set; } = new ObservableCollection<ILinkViewModel>();

        public LinksViewModel()
        {
            MessengerInstance.Register<MachineLoadMessage>(this, OnMachineLoadMessage);
        }

        private void OnMachineLoadMessage(MachineLoadMessage msg)
        {
            Links.Clear();

            foreach (var item in msg.Machine)
            {
                IterateMachineElementForLinks(item);
            }

            MessengerInstance.Send(new UpdateLinkViewModelsListMessage() { LinkViewModels = Links.ToList() }); 
        }

        private void IterateMachineElementForLinks(IMachineElementViewModel vm)
        {
            if (vm.LinkToParent != null) Links.Add(vm.LinkToParent);

            foreach (var item in vm.Children)
            {
                IterateMachineElementForLinks(item);
            }
        }
    }
}
