using HelixToolkit.Wpf.SharpDX;
using Transform3D = System.Windows.Media.Media3D.Transform3D;

namespace MachineElements.ViewModels.Interfaces
{
    public interface ISelectedMachineElementViewModel
    {
        int Id { get; }

        string Name { get; set; }

        Transform3D Transform { get; set; }

        Material Material { get; set; }
    }
}
