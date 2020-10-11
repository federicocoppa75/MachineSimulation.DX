using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.ToolHolder;
using System.Windows.Media.Media3D;

namespace MachineElements.ViewModels.Extensions
{
    public static class MachineElementViewModelExtennsions
    {
        public static Transform3D GetChainTansform(this IMachineElementViewModel vm)
        {
            if (vm != null)
            {
                var tg = new Transform3DGroup();

                AddParentTansform(vm, tg);

                return tg;
            }
            else
            {
                return null;
            }
        }

        private static void AddParentTansform(IMachineElementViewModel vm, Transform3DGroup tg)
        {
            tg.Children.Add(vm.Transform);

            if (vm.Parent != null)
            {
                AddParentTansform(vm.Parent, tg);                
            }
        }

        public static void ManageToolActivation(this IMachineElementViewModel vm, bool value)
        {
            if(vm is ToolHolderViewModel thvm)
            {
                thvm.ActiveTool = value;
            }
            else
            {
                foreach (var item in vm.Children)
                {
                    var child = item as MachineElementViewModel;

                    child.ManageToolActivation(value);
                }
            }
        }

        public static void RequestTreeviewVisibility(this IMachineElementViewModel vm)
        {
            if((vm.Parent != null) && (vm.Parent is IExpandibleElementViewModel evm) && !evm.IsExpanded)
            {
                RequestTreeviewVisibility(vm.Parent);
                evm.IsExpanded = true;
            }
        }
    }
}
