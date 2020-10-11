using MachineElements.ViewModels.Enums.Links.Interpolation;

namespace MachineViewer.Plugins.Common.Models.Links.Interpolation
{
    public class ArcComponentData
    {
        public int GroupId { get; set; }
        public double StartAngle { get; set; }
        public double Angle { get; set; }
        public double CenterCoordinate { get; set; }
        public double Radius { get; set; }
        public ArcComponent Component { get; set; }

        public ArcComponentData Clone() => new ArcComponentData()
        {
            GroupId = GroupId,
            StartAngle = StartAngle,
            Angle = Angle,
            CenterCoordinate = CenterCoordinate,
            Radius = Radius,
            Component = Component
        };
    }
}
