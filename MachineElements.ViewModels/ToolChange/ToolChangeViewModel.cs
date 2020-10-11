using GalaSoft.MvvmLight;
using MachineElements.ViewModels.Messages.ToolChange;
using System;
using System.Collections.ObjectModel;

namespace MachineElements.ViewModels.ToolChange
{
    public class ToolChangeViewModel : ViewModelBase
    {
        public ObservableCollection<ToolSinkViewModel> ToolSinks { get; set; } = new ObservableCollection<ToolSinkViewModel>();

        public ToolChangeViewModel() : base()
        {
            MessengerInstance.Register<UpdateAvailableToolSinkListMessage>(this, OnUpdateAvailableToolSinkListMessage);
            MessengerInstance.Register<ResetAvailableToolSinkListMessage>(this, OnResetAvailableToolSinkListMessage);
        }

        private void OnResetAvailableToolSinkListMessage(ResetAvailableToolSinkListMessage obj) => ToolSinks.Clear();

        private void OnUpdateAvailableToolSinkListMessage(UpdateAvailableToolSinkListMessage obj)
        {
            ToolSinks.Clear();
            MessengerInstance.Send(new GetAvailableToolSinkMessage() { SetAvailableToolSink = AddToolSink });
        }

        private void AddToolSink(int id, string name)
        {
            ToolSinks.Add(new ToolSinkViewModel() { Id = id, Name = name });
        }
    }
}
