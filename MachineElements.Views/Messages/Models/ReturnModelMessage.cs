using System;
using Element3D = HelixToolkit.Wpf.SharpDX.Element3D;

namespace MachineElements.Views.Messages.Models
{
    public class ReturnModelMessage
    {
        public object Destination { get; set; }
        public Action<object, Element3D> SetPair { get; set; }
    }
}
