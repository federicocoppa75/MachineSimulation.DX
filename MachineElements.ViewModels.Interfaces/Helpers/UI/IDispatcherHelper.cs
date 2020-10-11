using System;

namespace MachineElements.ViewModels.Interfaces.Helpers.UI
{
    public interface IDispatcherHelper
    {
        void CheckBeginInvokeOnUi(Action action);
    }
}
