using System;
using System.Windows.Media.Media3D;

namespace MachineElements.ViewModels.Messages.Panel
{
    public class GetPanelPositionMessage
    {
        public Action<bool, Transform3D, Rect3D?> SetData { get; set; }
    }
}
