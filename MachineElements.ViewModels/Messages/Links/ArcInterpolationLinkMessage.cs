using MachineViewer.Plugins.Common.Models.Links.Interpolation;

namespace MachineElements.ViewModels.Messages.Links
{
    public class ArcInterpolationLinkMessage : MoveLinearLinkMessage
    {
        public ArcComponentData ArcComponentData { get; private set; }

        public ArcInterpolationLinkMessage(int id, double targetCoordinate, double duration, ArcComponentData arcComponentData) : base(id, targetCoordinate, duration)
        {
            ArcComponentData = arcComponentData;
        }
    }
}
