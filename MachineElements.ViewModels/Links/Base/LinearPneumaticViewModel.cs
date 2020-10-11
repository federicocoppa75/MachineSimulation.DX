using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Interfaces.Enums;

namespace MachineElements.ViewModels.Links.Base
{
    public class LinearPneumaticViewModel : TwoPositionLinkViewModel
    {
        public override LinkType LinkType => LinkType.LinearPneumatic;

        public static LinearPneumaticViewModel Create() => new LinearPneumaticViewModel();
    }
}
