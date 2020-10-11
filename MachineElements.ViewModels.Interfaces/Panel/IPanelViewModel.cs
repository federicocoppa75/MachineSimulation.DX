using System.Windows.Media.Media3D;

namespace MachineElements.ViewModels.Interfaces.Panel
{
    public interface IPanelViewModel : IMachineElementViewModel
    {
        double SizeX { get; set; }
        double SizeY { get; set; }
        double SizeZ { get; set; }
        double CenterX { get; set; }
        double CenterY { get; set; }
        double CenterZ { get; set; }
        Rect3D Bound { get; }
        void Initialize();
    }
}
