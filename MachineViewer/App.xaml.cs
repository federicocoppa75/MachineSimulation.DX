using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using System.Windows;
using IDispatcherHelper = MachineElements.ViewModels.Interfaces.Helpers.UI.IDispatcherHelper;
using IPanelViewModel = MachineElements.ViewModels.Interfaces.Panel.IPanelViewModel;
using DispatcherHelperEx = MachineViewer.Helpers.UI.DispatcherHelperEx;
using PanelViewModel = MaterialRemoval.ViewModels.PanelViewModel;
//using PanelViewModel = MachineElements.ViewModels.Panel.PanelViewModel;
using MaterialRemovalMessageSender = MaterialRemoval.Helpers.MaterialRemovalMessageSender;
using IMaterialRemovalMessageSender = MachineElements.ViewModels.Interfaces.Panel.IMaterialRemovalMessageSender;
using IStepObserver = MachineElements.ViewModels.Interfaces.Steps.IStepObserver;
using StepObserver = MaterialRemoval.ViewModels.Steps.StepObserver;

namespace MachineViewer
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
            SimpleIoc.Default.Register<IDispatcherHelper, DispatcherHelperEx>();
            SimpleIoc.Default.Register<IPanelViewModel, PanelViewModel> ();
            SimpleIoc.Default.Register<IMaterialRemovalMessageSender, MaterialRemovalMessageSender>();
            SimpleIoc.Default.Register<IStepObserver, StepObserver>();
        }
    }
}
