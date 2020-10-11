using System;
using System.Windows.Media.Media3D;

namespace MachineElements.ViewModels.Messages.Panel
{
    public class GetPanelTransformMessage
    {
        //public Action<bool, Transform3D> SetData { get; set; }
        public Action<bool, Matrix3D> SetData { get; set; }
    }
}
