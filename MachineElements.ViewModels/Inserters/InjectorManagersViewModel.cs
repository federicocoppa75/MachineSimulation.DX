using GalaSoft.MvvmLight;
using MachineElements.ViewModels.Messages.Inserters;
using System;
using System.Collections.ObjectModel;

namespace MachineElements.ViewModels.Inserters
{
    public class InjectorManagersViewModel : ViewModelBase
    {
        public ObservableCollection<InjectorManagerViewModel> Injectors { get; set; } = new ObservableCollection<InjectorManagerViewModel>();

        public InjectorManagersViewModel() : base()
        {
            MessengerInstance.Register<UpdateAvailableInjectorsMessage>(this, OnUpdateAvailableInjectorsMessage);
            MessengerInstance.Register<ResetAvailableInjectorsMessage>(this, OnResetAvailableInjectorsMessage);
        }

        private void OnResetAvailableInjectorsMessage(ResetAvailableInjectorsMessage msg) => Injectors.Clear();

        private void OnUpdateAvailableInjectorsMessage(UpdateAvailableInjectorsMessage msg)
        {
            Injectors.Clear();

            MessengerInstance.Send(new GetAvailablaInjectorsMessage()
            {
                SetInjectorData = (id) =>
                {
                    Injectors.Add(new InjectorManagerViewModel()
                    {
                        Id = id
                    });
                }
            });
        }
    }

}
