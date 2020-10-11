using GalaSoft.MvvmLight;
using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Extensions;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Interfaces.Links;
using System.Collections.ObjectModel;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using PointProbeViewModel = MachineElements.ViewModels.Probing.PointProbeViewModel;
using MachineElements.ViewModels.Messages.Probe;
using ICommand = System.Windows.Input.ICommand;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;
using MachineElements.ViewModels.Interfaces.Messages;

namespace MachineElements.ViewModels
{
    public class MachineElementViewModel : ViewModelBase, IMachineElementViewModel, IExpandibleElementViewModel, IProbableElementViewModel
    {
        private static int _idSeed = 0;

        private static Material _preSelectionMaterial;

        public int Id { get; private set; }

        public string Name { get; set; }

        public IMachineElementViewModel Parent { get; set; }

        public ILinkViewModel LinkToParent { get; set; }

        public Geometry3D Geometry { get; set; }

        private Transform3D _transform;
        public Transform3D Transform 
        {
            get => (_transform != null) ? _transform : Transform3D.Identity;
            set => _transform = value; 
        }

        private Material _material;
        public Material Material 
        {
            get => _material;
            set => Set(ref _material, value, nameof(Material));
        }

        private bool _visibility;

        public bool Visible
        {
            get => _visibility;
            set => Set(ref _visibility, value, nameof(Visible)); 
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected; 
            set 
            { 
                if(Set(ref _isSelected, value, nameof(IsSelected)))
                {
                    PostEffects = _isSelected ? $"highlight[color:#FFFF00]" : null;
                    if (_isSelected) this.RequestTreeviewVisibility();
                    MessengerInstance.Send(new SelectMachineElementMessage() { Element = _isSelected ? this : null });
                }
            }
        }

        private string _postEffects;

        public string PostEffects
        {
            get => _postEffects; 
            set => Set(ref _postEffects, value, nameof(PostEffects)); 
        }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(ref _isExpanded, value, nameof(IsExpanded)); 
        }

        private ICommand _changeChainVisibilityState;
        public ICommand ChangeChainVisibilityState { get { return _changeChainVisibilityState ?? (_changeChainVisibilityState = new RelayCommand(() => ChangeChainVisibilityStateImpl())); } }


        public ObservableCollection<IMachineElementViewModel> Children { get; protected set; } = new ObservableCollection<IMachineElementViewModel>();

        public MachineElementViewModel()
        {
            Id = _idSeed++;
        }

        private void ChangeChainVisibilityStateImpl() => ChangeVisibleProperty(this, !Visible);

        private void ChangeVisibleProperty(MachineElementViewModel me, bool value)
        {
            me.Visible = value;
            ChangeChildrenVisibleProperty(me, value);
        }

        private void ChangeChildrenVisibleProperty(MachineElementViewModel me, bool value)
        {
            foreach (var item in me.Children)
            {
                if (item is MachineElementViewModel child)
                {
                    ChangeVisibleProperty(child, value);
                }
            }
        }

        public void AddProbePoint(Point3D point)
        {
            var probe = PointProbeViewModel.Create(this, point);

            Children.Add(probe);

            MessengerInstance.Send(new AddProbeMessage() { Probe = probe });
        }
    }
}
