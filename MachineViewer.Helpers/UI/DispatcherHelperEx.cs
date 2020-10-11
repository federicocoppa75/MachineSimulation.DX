using GalaSoft.MvvmLight.Threading;
using MachineElements.ViewModels.Interfaces.Helpers.UI;
using System;

namespace MachineViewer.Helpers.UI
{
    public class DispatcherHelperEx : IDispatcherHelper
    {
        public DispatcherHelperEx()
        {

        }

        public void CheckBeginInvokeOnUi(Action action) => DispatcherHelper.CheckBeginInvokeOnUI(action);
    }
}
