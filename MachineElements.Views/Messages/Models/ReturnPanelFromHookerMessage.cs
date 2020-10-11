using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Colliders;
using System;

namespace MachineElements.Views.Messages.Models
{
    public class ReturnPanelFromHookerMessage
    {
        public object PanelHooker { get; set; }
        public Action<Element3D> ReturnPanel { get; set; }
    }
}
