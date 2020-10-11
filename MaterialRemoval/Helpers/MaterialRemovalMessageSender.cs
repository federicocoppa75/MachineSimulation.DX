using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight.Messaging;
using MachineElements.ViewModels.Interfaces.Panel;
using MaterialRemoval.Messages;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace MaterialRemoval.Helpers
{
    public class MaterialRemovalMessageSender : IMaterialRemovalMessageSender
    {
        public void SendProcessPendingRemovalMessage() => Messenger.Default.Send(new ProcessPendingRemovalMessage());

        public void SendRoutToolMoveMessage(int toolId, Point3D position, Vector3D direction, double length, double radius)
        {
            Messenger.Default.Send(new RoutToolMoveMessage()
            {
                ToolId = toolId,
                Position = position,
                Direction = direction,
                Length = length,
                Radius = radius
            });
        }

        public void SendToolMoveMessage(int toolId, Point3D position, Vector3D direction, double length, double radius)
        {
            Messenger.Default.Send(new ToolMoveMessage()
            {
                Position = position,
                Direction = direction,
                Length = length,
                Radius = radius
            });
        }
    }
}
