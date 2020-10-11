using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Helpers;
using System.IO;
using Tools.Models;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Color4 = SharpDX.Color4;  
using System.Collections.Generic;
using System.Linq;

namespace MachineElements.ViewModels.Tools
{
    public class ToolViewModel : MachineElementViewModel
    {
        protected ToolViewModel() : base()
        {
        }

        public static ToolViewModel Create(Tool tool, Point3D position, Vector3D direction)
        {
            var vm = new ToolViewModel();

            vm.Name = tool.Name;
            vm.Material = PhongMaterials.Blue;
            vm.Geometry = ToolsHelpers.GetToolModel(tool, position, direction);
            vm.Visible = true;
            vm.PostEffects = "xrayGrid[color:#D22FDCDC]";

            return vm; 
        }
    }
}
