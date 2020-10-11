using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Extensions;
using MachineElements.ViewModels.Helpers;
using System;
using System.Windows.Media.Media3D;
using IMachineElementViewModel = MachineElements.ViewModels.Interfaces.IMachineElementViewModel;
using PhongMaterials = HelixToolkit.Wpf.SharpDX.PhongMaterials;

namespace MachineElements.ViewModels.Probing
{
    public class PointProbeViewModel : ProbeViewModel
    {
        //private Action<bool> _onIsSelectedChanged;

        public double Radius { get; set; }

        public override ProbeType ProbeType => ProbeType.Point;

        public static PointProbeViewModel Create(IMachineElementViewModel parent, Point3D point, double radius = 5.0)
        {
            var t = parent.GetChainTansform();
            var p = t.Inverse.Transform(point);
            //var builder = new MeshBuilder();

            //builder.AddSphere(p, radius);

            //var vm = new PointProbeViewModel()
            //{
            //    X = p.X,
            //    Y = p.Y,
            //    Z = p.Z,
            //    Radius = radius,
            //    Parent = parent,
            //    MeshGeometry = builder.ToMesh(),
            //    Fill = Brushes.Yellow
            //};

            //vm._onIsSelectedChanged = (b) => vm.Fill = b ? Brushes.Red : Brushes.Yellow;
            //vm.PropertyChanged += (s, e) => vm.OnPropertyChanged(s, e);

            //return vm;
            //return null;

            var probe = new PointProbeViewModel()
            {
                Parent = parent,
                Geometry = ProbesHelper.GetProbePointModel(p, radius),
                Material = PhongMaterials.Yellow,
                Visible = true,
                X = p.X,
                Y = p.Y,
                Z = p.Z
            };

            probe.Name = $"Probe point {probe.Id}";

            return probe;
        }

        //private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (string.Compare(e.PropertyName, "IsSelected") == 0) _onIsSelectedChanged?.Invoke(IsSelected);
        //}
    }
}
