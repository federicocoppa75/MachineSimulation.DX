using MachineElements.ViewModels.Links;
using MachineModels.Models.Links;
using System;
using VmLinkDirection = MachineElements.ViewModels.Interfaces.Enums.LinkDirection;
using MLinkDirection = MachineModels.Enums.LinkDirection;
using MachineElements.ViewModels.Interfaces.Links;

#if false
using LinearPositionViewModel = MachineElements.ViewModels.Links.Base.LinearPositionViewModel;
using RotaryPneumaticViewModel = MachineElements.ViewModels.Links.Base.RotaryPneumaticViewModel;
using LinearPneumaticViewModel = MachineElements.ViewModels.Links.Base.LinearPneumaticViewModel;
#else
using LinearPositionViewModel = MachineElements.ViewModels.Links.Evo.LinearPositionViewModel;
using RotaryPneumaticViewModel = MachineElements.ViewModels.Links.Evo.RotaryPneumaticViewModel;
using LinearPneumaticViewModel = MachineElements.ViewModels.Links.Evo.LinearPneumaticViewModel;
#endif


namespace MachineElement.Model.IO.Extensions
{
    public static class LinkExtension
    {
        public static ILinkViewModel Convert(this ILink link)
        {
            if (link == null) return null;

            if (link is LinearPosition linPos)
            {
                return UpdateViewModel(LinearPositionViewModel.Create(), linPos);
            }
            else if (link is LinearPneumatic pnmPos)
            {
                return UpdateViewModel(LinearPneumaticViewModel.Create(), pnmPos);
            }
            else if (link is RotaryPneumatic pnmRot)
            {
                return UpdateViewModel(RotaryPneumaticViewModel.Create(), pnmRot);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static LinearPositionViewModel UpdateViewModel(LinearPositionViewModel vm, LinearPosition m)
        {
            vm.Id = m.Id;
            vm.Max = m.Max;
            vm.Min = m.Min;
            vm.Pos = m.Pos;
            vm.Direction = ConvertLinkDirection(m.Direction);
            vm.Value = m.Pos;

             return vm;
        }

        public static TwoPositionLinkViewModel UpdateViewModel(TwoPositionLinkViewModel vm, TwoPositionsLink m)
        {
            vm.Id = m.Id;
            vm.OffPos = m.OffPos;
            vm.OnPos = m.OnPos;
            vm.TOff = m.TOff;
            vm.TOn = m.TOn;
            vm.Direction = ConvertLinkDirection(m.Direction);
            vm.ToolActivator = m.ToolActivator;

            return vm;
        }

        private static VmLinkDirection ConvertLinkDirection(MLinkDirection direction)
        {
            switch (direction)
            {
                case MLinkDirection.X:
                    return VmLinkDirection.X;

                case MLinkDirection.Y:
                    return VmLinkDirection.Y;
                    
                case MLinkDirection.Z:
                    return VmLinkDirection.Z;

                default:
                    throw new ArgumentException("Invalid link direction");
            }
        } 
    }
}
