using Point3D = System.Windows.Media.Media3D.Point3D;

namespace MachineElements.ViewModels.Interfaces
{
    public interface IProbableElementViewModel
    {
        void AddProbePoint(Point3D point);
    }
}
