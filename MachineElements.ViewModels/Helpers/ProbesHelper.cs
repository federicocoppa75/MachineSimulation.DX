using Point3D = System.Windows.Media.Media3D.Point3D;
using Geometry3D = HelixToolkit.Wpf.SharpDX.Geometry3D;
using MeshBuilder = HelixToolkit.Wpf.SharpDX.MeshBuilder;
using HelixToolkit.Wpf.SharpDX;
using System.Linq;

namespace MachineElements.ViewModels.Helpers
{
    public static class ProbesHelper
    {
        public static Geometry3D GetProbePointModel(Point3D position, double radius)
        {
            var builder = new MeshBuilder();

            builder.AddSphere(position.ToVector3(), radius);

            return builder.ToMesh();
        }

        public static Geometry3D GetProbeDistanceModel(Point3D[] points)
        {
            var builder = new LineBuilder();

            builder.Add(false, points.Select(i => i.ToVector3()).ToArray());

            return builder.ToLineGeometry3D();
        }
    }
}
