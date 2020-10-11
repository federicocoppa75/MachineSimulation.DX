using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CncViewer.Connection.Enums;
using CncViewer.Connection.Messages;
using GalaSoft.MvvmLight.Command;

namespace CncViewer.Connection.ViewModels.Links
{
    public class PulseTwoPosLinkViewModel : LinkViewModel<bool>
    {
        public override LinkType Type => LinkType.PulseTwoPos;

        private ICommand _pulseCommand;
        public ICommand PulseCommand { get { return _pulseCommand ?? (_pulseCommand = new RelayCommand(PulseCommandImplementation)); } }

        public PulseTwoPosLinkViewModel(int id, string variable) : base(id, variable)
        {
        }

        private async void PulseCommandImplementation()
        {
            await Task.Run(() => MessengerInstance.Send(new WriteValueMessage<bool>() { LinkId = Id, Value = true }));
            await Task.Delay(500);
            await Task.Run(() => MessengerInstance.Send(new WriteValueMessage<bool>() { LinkId = Id, Value = false }));
        }
    }
}
