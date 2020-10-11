using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Colliders;
using System;

namespace MachineElements.Views.Messages.Models
{
    public class TransferPanelToHookerMessage
    {
        public object PanelHooker { get; set; }
        public Func<Element3D> TransferPanel { get; set; }
    }
}
