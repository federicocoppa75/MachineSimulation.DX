using System;
using System.Windows.Media.Media3D;
using Tools.Models;

namespace MachineElements.ViewModels.Messages.ToolState
{
    public class GetActiveRoutToolMessage
    {
        public Action<Point3D, Vector3D, Tool, int> SetData { get; set; }
    }
}
