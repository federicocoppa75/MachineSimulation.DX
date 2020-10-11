using HelixToolkit.Wpf.SharpDX;
using System.Windows.Media.Media3D;

namespace MachineElements.ViewModels.Extensions
{
    public static class Rect3DExtensions
    {
        public static Rect3D ToRect3D(this SharpDX.BoundingBox box) => new Rect3D(box.Minimum.ToPoint3D(), box.Size.ToSize3D());
    }
}
