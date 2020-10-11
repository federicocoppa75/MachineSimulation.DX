using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace MachineElements.ViewModels.Interfaces.Panel
{
    public interface IMaterialRemovalMessageSender
    {
        void SendProcessPendingRemovalMessage();
        void SendRoutToolMoveMessage(int toolId, Point3D position, Vector3D direction, double length, double radius);
        void SendToolMoveMessage(int toolId, Point3D position, Vector3D direction, double length, double radius);
    }
}
