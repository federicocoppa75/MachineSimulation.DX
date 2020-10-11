using System.Windows.Media.Media3D;
using Material = HelixToolkit.Wpf.SharpDX.Material;

namespace MachineElements.ViewModels.Messages.Inserters
{
    public class InjectMessage
    {
        public Point3D Position { get; set; }
        public Vector3D Direction { get; set; }
        public Material Material { get; set; }
    }
}
