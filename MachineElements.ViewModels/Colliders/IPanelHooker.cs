using System.Windows.Media.Media3D;
//using PanelViewModel = MachineElements.ViewModels.Panel.PanelViewModel;
using IPanelViewModel = MachineElements.ViewModels.Interfaces.Panel.IPanelViewModel;

namespace MachineElements.ViewModels.Colliders
{
    public interface IPanelHooker
    {
        Transform3D TotalTransformation { get; }

        void HookPanel(IPanelViewModel panel);

        IPanelViewModel UnhookPanel();
    }
}
