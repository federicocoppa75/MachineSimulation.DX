using GalaSoft.MvvmLight;
using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Messages.MenuCommands;
using System.Collections.ObjectModel;
using Color = System.Windows.Media.Color;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using BoundingBox = SharpDX.BoundingBox;
using System;
using LightType = MachineElements.ViewModels.Interfaces.Enums.LightType;
using MachineElements.ViewModels.Lights;
using MachineElements.ViewModels.Interfaces.Messages;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using MachineElements.ViewModels.Interfaces.Links;
using Size = System.Windows.Size;

namespace MachineElements.ViewModels
{
    public class MachineViewModel : ViewModelBase, IMachineViewModel
    {
        class DummyStepExecutionInfoProvider : IStepExecutionInfoProvider
        {
            private readonly ObservableCollection<IUpdatableValueLink<double>> _linearPositionLinks = new ObservableCollection<IUpdatableValueLink<double>>();
            
            public bool IsStepTimeVisible => false;
            public bool IsAxesStateVisible => false;
            public ObservableCollection<IUpdatableValueLink<double>> LinearPositionLinks => _linearPositionLinks;
            public TimeSpan StepTime => TimeSpan.FromSeconds(0.0); 
            public string InverterName => string.Empty;
            public int InverterValue => 0;
            public bool IsInverterStateVisible => false;
        }

        private static DummyStepExecutionInfoProvider _dummyStepExecutionInfoProvider = new DummyStepExecutionInfoProvider();

        public EffectsManager EffectsManager { get; }
        public Camera Camera { get; }
        public Color AmbientLightColor { get; private set; }

        private Point3D _fixedRotationPoint;
        public Point3D FixedRotationPoint 
        {
            get => _fixedRotationPoint;
            set 
            {
                if(!_fixedRotationPoint.Equals(value))
                {
                    _fixedRotationPoint = value;
                    RaisePropertyChanged(nameof(FixedRotationPoint));
                }
            }
        }

        private LightType _lightType;
        public LightType LightType
        {
            get => _lightType;
            set
            {
                if(Set(ref _lightType, value, nameof(LightType)))
                {
                    SetLights();
                }
            }
        }

        private bool _showFrameDetails;
        public bool ShowFrameDetails
        {
            get => _showFrameDetails; 
            set => Set(ref _showFrameDetails, value, nameof(ShowFrameDetails));
        }

        private bool _showTriangleCountInfo;
        public bool ShowTriangleCountInfo
        {
            get => _showTriangleCountInfo; 
            set => Set(ref _showTriangleCountInfo, value, nameof(ShowTriangleCountInfo)); 
        }

        private bool _showFrameRate;
        public bool ShowFrameRate
        {
            get => _showFrameRate; 
            set => Set(ref _showFrameRate, value, nameof(ShowFrameRate)); 
        }

        private bool _showCameraInfo;
        public bool ShowCameraInfo
        {
            get => _showCameraInfo;
            set => Set(ref _showCameraInfo, value, nameof(ShowCameraInfo));
        }

        private bool _enableSelectionByVIew;

        public bool EnableSelectionByView
        {
            get => _enableSelectionByVIew; 
            set => Set(ref _enableSelectionByVIew, value, nameof(EnableSelectionByView));
        }

        private bool _addProbePoint;

        public bool AddProbePoint
        {
            get => _addProbePoint;
            set => Set(ref _addProbePoint, value, nameof(AddProbePoint));
        }

        private IStepExecutionInfoProvider _stepExecutionInfoProvider;
        public IStepExecutionInfoProvider StepExecutionInfoProvider
        {
            get => (_stepExecutionInfoProvider != null) ? _stepExecutionInfoProvider : _dummyStepExecutionInfoProvider;
            set => Set(ref _stepExecutionInfoProvider, value, nameof(StepExecutionInfoProvider)); 
        }

        private Color _backgroundStartColor;
        public Color BackgroundStartColor
        {
            get => _backgroundStartColor;
            set => Set(ref _backgroundStartColor, value, nameof(BackgroundStartColor));
        }

        private Color _backgroundStopColor;
        public Color BackgroundStopColor
        {
            get => _backgroundStopColor;
            set => Set(ref _backgroundStopColor, value, nameof(BackgroundStopColor));
        }

        public ObservableCollection<IMachineElementViewModel> Machines { get; set; } = new ObservableCollection<IMachineElementViewModel>();
        public ObservableCollection<AmbientLightViewModel> AmbientLights { get; set; } = new ObservableCollection<AmbientLightViewModel>();
        public ObservableCollection<DirectionalLightViewModel> DirectionalLights { get; set; } = new ObservableCollection<DirectionalLightViewModel>();
        public ObservableCollection<DirectionalLightViewModel> DirectionalOrientedByCameraLights { get; set; } = new ObservableCollection<DirectionalLightViewModel>();
        public ObservableCollection<SpotLightViewModel> SpotLights { get; set; } = new ObservableCollection<SpotLightViewModel>();

        private ICommand _selectItemCommand;
        public ICommand SelectItemCommand { get { return _selectItemCommand ?? (_selectItemCommand = new RelayCommand(() => SelectItemCommandImplementation())); } }

        private void SelectItemCommandImplementation()
        {
            throw new NotImplementedException();
        }

        public MachineViewModel()
        {
            BackgroundStartColor = System.Windows.Media.Colors.Gray;
            BackgroundStopColor = System.Windows.Media.Colors.White;
            AmbientLightColor = System.Windows.Media.Colors.Gray;
            EffectsManager = new DefaultEffectsManager();
            Camera = new PerspectiveCamera() { Position = new Point3D(0, 0, 3000), LookDirection = new Vector3D(0, 0, -200), UpDirection = new Vector3D(0, 1, 0), FarPlaneDistance = 10000, NearPlaneDistance = 1 }; ;

            MessengerInstance.Register<MachineLoadMessage>(this, OnMachineLoadMessage);
            MessengerInstance.Register<GetMachineViewModel>(this, OnGetMachineViewModel);

            SetLights();
        }

        private void OnGetMachineViewModel(GetMachineViewModel msg) => msg?.SetAction(this);

        private void OnMachineLoadMessage(MachineLoadMessage msg)
        {
            Machines.Clear();

            foreach (var item in msg.Machine)
            {
                Machines.Add(item);
            }

            if(Machines.Count > 0) ResetCamera();
        }

        private void ResetCamera()
        {
            var box = GetBoundingBox();
            var size = box.Size;
            var diagonal = new Vector3D(size.X, size.Y, size.Z);
            var center = box.Center();
            double radius = diagonal.Length * 0.5;

            if (Camera is PerspectiveCamera)
            {
                var perspectiveCamera = Camera as PerspectiveCamera;
                var pcam = perspectiveCamera;
                double disth = radius / Math.Tan(0.5 * pcam.FieldOfView * Math.PI / 180);
                double vfov = pcam.FieldOfView;
                double distv = radius / Math.Tan(0.5 * vfov * Math.PI / 180);
                double dist = Math.Max(disth, distv);
                var newLookDirection = (new Vector3D(1.0, 1.0, -1.0)) * dist;
                var newPosition = center.ToPoint3D() - newLookDirection;

                Camera.Position = newPosition;
                Camera.LookDirection = newLookDirection;
                Camera.UpDirection = new Vector3D(0.0, 0.0, 1.0);
            }
        }

        private BoundingBox GetBoundingBox() => GetBoundingBox(Machines[0]);

        private BoundingBox GetBoundingBox(IMachineElementViewModel me)
        {
            var box = (me.Geometry != null) ? me.Geometry.Bound : new BoundingBox();

            foreach (var item in me.Children)
            {
                var childBox = GetBoundingBox(item);

                box = BoundingBox.Merge(box, childBox);
            }

            var p1 = me.Transform.Transform(box.Minimum.ToPoint3D());
            var p2 = me.Transform.Transform(box.Maximum.ToPoint3D());

            box.Minimum = p1.ToVector3();
            box.Maximum = p2.ToVector3();

            return box;
        }

        private void SetLights()
        {
            switch (_lightType)
            {
                case LightType.Default:
                    SetDefaultLights();
                    break;
                case LightType.Sun:
                    SetSunLighs();
                    break;
                case LightType.Spot:
                    SetSpotLight();
                    break;
                case LightType.Default2:
                    SetDefaultLights2();
                    break;
                case LightType.Default3:
                    SetDefaultLights3();
                    break;
                default:
                    break;
            }
        }

        private void ClearLights()
        {
            DirectionalOrientedByCameraLights.Clear();
            AmbientLights.Clear();
            DirectionalLights.Clear();
            SpotLights.Clear();
        }

        private void SetDefaultLights()
        {
            ClearLights();

            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(60, 60, 60), Direction = new Vector3D(0.1, 1, -1) });
            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(105, 105, 105), Direction = new Vector3D(-1, -1, -1) });
            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(128, 128, 128), Direction = new Vector3D(1, -1, -0.1) });
            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(50, 50, 50), Direction = new Vector3D(-1, -1, -1) });
            AmbientLights.Add(new AmbientLightViewModel() { Color = Color.FromRgb(30, 30, 30) });
        }

        private void SetDefaultLights2()
        {
            ClearLights();

            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(60, 60, 60), Direction = new Vector3D(0.1, -1, -1) });
            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(105, 105, 105), Direction = new Vector3D(-1, 1, -1) });
            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(128, 120, 128), Direction = new Vector3D(1, 1, -0.1) });
            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(50, 50, 50), Direction = new Vector3D(-1, 1, -1) });
            AmbientLights.Add(new AmbientLightViewModel() { Color = Color.FromRgb(30, 30, 30) });
        }

        private void SetDefaultLights3()
        {
            ClearLights();

            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(60, 60, 60), Direction = new Vector3D(0.1, 1, -1) });
            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(105, 105, 105), Direction = new Vector3D(-1, -1, -1) });
            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(128, 128, 128), Direction = new Vector3D(1, -1, 1) });
            DirectionalLights.Add(new DirectionalLightViewModel() { Color = Color.FromRgb(50, 50, 50), Direction = new Vector3D(-1, -1, 1) });
            AmbientLights.Add(new AmbientLightViewModel() { Color = Color.FromRgb(30, 30, 30) });
        }

        private void SetSunLighs()
        {
            ClearLights();

            DirectionalOrientedByCameraLights.Add(new DirectionalLightViewModel() { Color = System.Windows.Media.Colors.WhiteSmoke });
            AmbientLights.Add(new AmbientLightViewModel() { Color = Color.FromRgb(22, 22, 22) });

        }


        private void SetSpotLight()
        {
            ClearLights();

            SpotLights.Add(new SpotLightViewModel() { Color = System.Windows.Media.Colors.WhiteSmoke });
            AmbientLights.Add(new AmbientLightViewModel() { Color = System.Windows.Media.Colors.Black });
        }
    }
}
