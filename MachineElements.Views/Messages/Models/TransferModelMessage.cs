using Element3D = HelixToolkit.Wpf.SharpDX.Element3D;

namespace MachineElements.Views.Messages.Models
{
    public class TransferModelMessage
    {
        public object Destination { get; set; }
        public object Key { get; set; }
        public Element3D Value { get; set; }
    }
}
