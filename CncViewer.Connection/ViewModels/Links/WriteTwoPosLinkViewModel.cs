using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CncViewer.Connection.Enums;
using CncViewer.Connection.Messages;

namespace CncViewer.Connection.ViewModels.Links
{
    public class WriteTwoPosLinkViewModel : LinkViewModel<bool>
    {
        public override LinkType Type => LinkType.WriteTwoPos;

        public bool ValueToWrite { get; set; }

        private ICommand _writeCommand;
        public ICommand WriteCommand { get { return _writeCommand ?? (_writeCommand = new GalaSoft.MvvmLight.Command.RelayCommand(WriteCommandImplementation)); } }

        public WriteTwoPosLinkViewModel(int id, string variable) : base(id, variable)
        {
        }
        
        private async void WriteCommandImplementation()
        {
            await Task.Run(() => MessengerInstance.Send(new WriteValueMessage<bool>() { LinkId = Id, Value = ValueToWrite }));
        }
    }
}
