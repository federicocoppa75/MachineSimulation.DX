using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Interfaces.Enums;

namespace MachineElements.ViewModels.Links.Base
{
    public class RotaryPneumaticViewModel : TwoPositionLinkViewModel
    {
        public override LinkType LinkType => LinkType.RotaryPneumatic;

        public static RotaryPneumaticViewModel Create() => new RotaryPneumaticViewModel();
    }
}
