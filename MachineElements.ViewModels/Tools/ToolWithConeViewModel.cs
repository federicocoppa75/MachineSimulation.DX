using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Helpers;
using Tools.Models;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;

namespace MachineElements.ViewModels.Tools
{
    public class ToolWithConeViewModel : MachineElementViewModel
    {
        protected ToolWithConeViewModel() : base()
        {
        }

        public static ToolWithConeViewModel Create(Tool tool, Point3D position, Vector3D direction)
        {
            var vm = new ToolWithConeViewModel();
            var tvm = ToolViewModel.Create(tool, new Point3D(), direction);

            vm.Name = $"{tool.Name}(cone)";
            vm.Material = PhongMaterials.PolishedSilver;
            vm.Geometry = ToolsHelpers.GetConeModel(tool.ConeModelFile);
            vm.Transform = new TranslateTransform3D() { OffsetX = position.X, OffsetY = position.Y, OffsetZ = position.Z };
            vm.Visible = true;
            vm.Children.Add(tvm);
            tvm.Parent = vm;

            return vm;
        }
    }
}
