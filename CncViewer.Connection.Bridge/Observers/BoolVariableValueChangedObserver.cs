using CncViewer.Connection.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using MachineElements.ViewModels.Helpers.UI;
using MachineElements.ViewModels.Messages.Links;

namespace CncViewer.Connection.Bridge.Observers
{
    public class BoolVariableValueChangedObserver : IVariableValueChangedObserver<bool>
    {
        public void ValueChanged(int linkId, bool value)
        {
            DispatcherHelperEx.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send(new UpdateTwoPositionLinkStateMessage(linkId, value));
            });            
        }
    }
}
