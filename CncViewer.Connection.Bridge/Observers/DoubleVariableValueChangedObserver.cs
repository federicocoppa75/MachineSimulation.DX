using CncViewer.Connection.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using MachineElements.ViewModels.Helpers.UI;
using MachineElements.ViewModels.Messages.Links;

namespace CncViewer.Connection.Bridge.Observers
{
    public class DoubleVariableValueChangedObserver : IVariableValueChangedObserver<double>
    {
        public void ValueChanged(int linkId, double value)
        {
            DispatcherHelperEx.CheckBeginInvokeOnUI(() =>
            {
                Messenger.Default.Send(new UpdateLinearLinkStateMessage(linkId, value));
            });            
        }
    }
}
