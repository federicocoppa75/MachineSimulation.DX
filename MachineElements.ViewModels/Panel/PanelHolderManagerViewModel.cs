using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MachineElements.Models.Panel;
using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Messages.PanelHolder;
using System;
using System.Windows.Input;

namespace MachineElements.ViewModels.Panel
{
    public class PanelHolderManagerViewModel : ViewModelBase
    {
        private bool _panelHold = false;

        public bool PanelHold
        {
            get => _panelHold;
            private set
            {
                if (Set(ref _panelHold, value, nameof(PanelHold))) UpdateCanExecuteCommands();
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public PanelLoadType Corner { get; set; }

        private ICommand _loadPanel;
        public ICommand LoadPanel => _loadPanel ?? (_loadPanel = new RelayCommand(LoadPanelImpl, CanExecuteLoadPanel));

        private ICommand _unloadPanel;
        public ICommand UnloadPanel => _unloadPanel ?? (_unloadPanel = new RelayCommand(UnloadPanelImpl, CanExecuteUnloadPanel));

        private void LoadPanelImpl()
        {
            PanelData panel = null;

            MessengerInstance.Send(new GetPanelDataMessage() { SetPanelData = (d) => panel = d });

            if (panel != null)
            {
                MessengerInstance.Send(new LoadPanelMessage()
                {
                    PanelHolderId = Id,
                    Length = panel.Length,
                    Width = panel.Width,
                    Height = panel.Height,
                    NotifyExecution = (b) => PanelHold = b
                });              
                
            }
            else
            {
                throw new InvalidOperationException("Panel data must not be null!");
            }
        }

        private bool CanExecuteLoadPanel() => !_panelHold;

        private void UnloadPanelImpl()
        {
            MessengerInstance.Send(new UnloadPanelMessage()
            {
                PanelHolderId = Id,
                NotifyExecution = (b) => PanelHold = !b
            });
        }

        private bool CanExecuteUnloadPanel() => _panelHold;

        private void UpdateCanExecuteCommands()
        {
            (_loadPanel as RelayCommand)?.RaiseCanExecuteChanged();
            (_unloadPanel as RelayCommand)?.RaiseCanExecuteChanged();
        }
    }

}
