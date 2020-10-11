using GalaSoft.MvvmLight;
using MachineElements.Models.Panel;
using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Messages.PanelHolder;
using System;
using System.Collections.ObjectModel;

namespace MachineElements.ViewModels.Panel
{
    public class PanelHoldersManagerViewModel : ViewModelBase
    {
        public ObservableCollection<PanelHolderManagerViewModel> PanelHolders { get; set; } = new ObservableCollection<PanelHolderManagerViewModel>();

        public PanelData PanelData { get; set; } = new PanelData { Length = 800.0, Width = 600.0, Height = 18.0 };

        public PanelHoldersManagerViewModel() : base()
        {
            MessengerInstance.Register<UpdateAvailablePanelHolderMessage>(this, OnUpdateAvailablePanelHolderMessage);
            MessengerInstance.Register<ResetAvailablePanelHolderMessage>(this, OnResetAvailablePanelHolderMessage);
            MessengerInstance.Register<GetPanelDataMessage>(this, OnGetPanelDataMessage);
        }

        private void OnResetAvailablePanelHolderMessage(ResetAvailablePanelHolderMessage obj) => PanelHolders.Clear();

        private void OnGetPanelDataMessage(GetPanelDataMessage msg) => msg.SetPanelData(PanelData);

        private void OnUpdateAvailablePanelHolderMessage(UpdateAvailablePanelHolderMessage obj)
        {
            PanelHolders.Clear();

            MessengerInstance.Send(new GetAvailablePanelHoldersMessage() { AvailableToolHolder = AddPanelHolder });
        }

        private void AddPanelHolder(int id, string name, PanelLoadType corner)
        {
            PanelHolders.Add(new PanelHolderManagerViewModel() { Id = id, Name = name, Corner = corner });
        }
    }
}
