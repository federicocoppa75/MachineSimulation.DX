using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Extensions;
using MachineElements.ViewModels.Helpers;
using System;
using System.Windows.Media.Media3D;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace MachineElements.ViewModels.Probing
{
    public class PointsDistanceViewModel : ProbeViewModel
    {
        //private PointProbeViewModel _vm1;
        //private PointProbeViewModel _vm2;
        private Action<bool> _onIsSelectedChanged;

        public override ProbeType ProbeType => ProbeType.Distance;

        public double Thickness { get; set; }

        public double Smoothness { get; set; }

        public Color Color { get; set; }

        public static PointsDistanceViewModel Create(PointProbeViewModel vm1, PointProbeViewModel vm2)
        {
            var p1 = new Point3D(vm1.X, vm1.Y, vm1.Z);
            var p2 = new Point3D(vm2.X, vm2.Y, vm2.Z);
            var t1 = vm1.GetChainTansform();
            var t2 = vm2.GetChainTansform();
            var pp2 = t1.Inverse.Transform(t2.Transform(p2));
            var p12 = pp2 - p1;

            var points = new Point3D[]
            {
                p1,
                p1 + new Vector3D(p12.X, 0.0, 0.0),
                p1 + new Vector3D(p12.X, p12.Y, 0.0),
                pp2
            };

            var pdvm = new PointsDistanceViewModel()
            {
                X = p12.X,
                Y = p12.Y,
                Z = p12.Z,
                //_vm1 = vm1,
                //_vm2 = vm2,
                Parent = vm1,
                Visible = true,
                Geometry = ProbesHelper.GetProbeDistanceModel(points),
                Thickness = 1.0,
                Smoothness = 1.0,
                Color = Colors.Yellow
            };

            pdvm.Name = $"Probe distance {pdvm.Id}";

            return pdvm;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(string.Compare(e.PropertyName, "IsSelected") == 0) _onIsSelectedChanged?.Invoke(IsSelected);
        }
    }
}
