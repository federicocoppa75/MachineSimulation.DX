using MachineElements.ViewModels;
using System.Windows;
using System.Windows.Controls;
using Element3D = HelixToolkit.Wpf.SharpDX.Element3D;
using GeometryModel3D = HelixToolkit.Wpf.SharpDX.GeometryModel3D;
using BitmapExtensions = HelixToolkit.Wpf.SharpDX.BitmapExtensions;
using Direct2DImageFormat = HelixToolkit.Wpf.SharpDX.Direct2DImageFormat;
using Vector2 = SharpDX.Vector2;
using Color = SharpDX.Color;
using HelixToolkit.Wpf.SharpDX;
using IProbableElementViewModel = MachineElements.ViewModels.Interfaces.IProbableElementViewModel;

namespace MachineElements.Views
{
    /// <summary>
    /// Logica di interazione per MachineView.xaml
    /// </summary>
    public partial class MachineView : UserControl
    {
        private GeometryModel3D _selectedModel;

        public MachineView()
        {
            InitializeComponent();

            var machineViewModel = new MachineViewModel();
            DataContext = machineViewModel;

            view3DX.AddHandler(Element3D.MouseDown3DEvent, new RoutedEventHandler((s, e) =>
            {
                var arg = e as HelixToolkit.Wpf.SharpDX.MouseDown3DEventArgs;

                if (arg.HitTestResult == null) return;

                if (machineViewModel.EnableSelectionByView)
                {
                    var selectedModel = arg.HitTestResult.ModelHit as GeometryModel3D;
                    var updateSelection = true;


                    if (_selectedModel != null)
                    {
                        updateSelection = !ReferenceEquals(selectedModel, _selectedModel);
                        _selectedModel.IsSelected = false;
                        _selectedModel = null;
                    }

                    if (updateSelection)
                    {
                        selectedModel.IsSelected = true;
                        _selectedModel = selectedModel;
                    }
                }
                else if(machineViewModel.AddProbePoint)
                {
                    var selectedModel = arg.HitTestResult.ModelHit as GeometryModel3D;
                    var point = arg.HitTestResult.PointHit.ToPoint3D();
                    var vm = selectedModel.DataContext as IProbableElementViewModel;

                    vm?.AddProbePoint(point);
                }
            }));

        }

     }
}
