using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight;
using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Interfaces.Links;
using Geometry3D = HelixToolkit.Wpf.SharpDX.Geometry3D;
using Material = HelixToolkit.Wpf.SharpDX.Material;

namespace MaterialRemoval.ViewModels.Base
{
    public abstract class PanelElementViewModel : ViewModelBase, IMachineElementViewModel
    {
        private static int _seedId = 0;

        public int Id { get; private set; }

        public string Name { get; set; }

        public IMachineElementViewModel Parent { get; set; }

        public ILinkViewModel LinkToParent { get; set; }

        private Geometry3D _geometry;
        public Geometry3D Geometry 
        { 
            get => _geometry;
            set => Set(ref _geometry, value, nameof(Geometry));
        }

        private Transform3D _transform;
        public Transform3D Transform
        {
            get => (_transform != null) ? _transform : Transform3D.Identity;
            set => _transform = value;
        }
        public Material Material { get; set; }
        
        public bool Visible { get; set; }
        
        public bool IsSelected { get; set; }

        public abstract ObservableCollection<IMachineElementViewModel> Children { get; }

        public PanelElementViewModel() : base()
        {
            Id = _seedId++;
        }
    }
}
