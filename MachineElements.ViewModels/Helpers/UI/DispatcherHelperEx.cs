using GalaSoft.MvvmLight.Ioc;
using MachineElements.ViewModels.Interfaces.Helpers.UI;
using System;

namespace MachineElements.ViewModels.Helpers.UI
{
    public static class DispatcherHelperEx
    {
        private static IDispatcherHelper _dispatcherHelper = null;

        public static void CheckBeginInvokeOnUI(Action action)
        {
            var dispatcherHelper = (_dispatcherHelper ?? SimpleIoc.Default.GetInstance<IDispatcherHelper>());

            dispatcherHelper.CheckBeginInvokeOnUi(action);
        }
    }
}
