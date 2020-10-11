using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Interfaces.Links;
using System.Collections.ObjectModel;
using Transform3D = System.Windows.Media.Media3D.Transform3D;

namespace MachineElements.ViewModels.Interfaces
{
    public interface IMachineElementViewModel
    {
        int Id { get; }

        string Name { get; set; }

        IMachineElementViewModel Parent { get; set; }

        ILinkViewModel LinkToParent { get; set; }

        Geometry3D Geometry { get; set; }

        Transform3D Transform { get; set; }

        Material Material { get; set; }

        bool Visible { get; set; }

        bool IsSelected { get; set; }

        ObservableCollection<IMachineElementViewModel> Children { get; }
    }
}
