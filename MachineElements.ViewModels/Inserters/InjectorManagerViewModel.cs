using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MachineElements.ViewModels.Messages.Inserters;
using System.Windows.Input;

namespace MachineElements.ViewModels.Inserters
{
    public class InjectorManagerViewModel : ViewModelBase
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => Set(ref _id, value, nameof(Id));
        }

        private ICommand _inject;
        public ICommand Inject => _inject ?? (_inject = new RelayCommand(InjectImpl));

        public InjectorManagerViewModel() : base()
        {

        }

        private void InjectImpl() => MessengerInstance.Send(new ExecuteInjectionMessage() { Id = Id });
    }

}
