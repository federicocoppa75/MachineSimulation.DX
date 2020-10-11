using MachineElements.ViewModels.Enums;

namespace MachineElements.ViewModels.Probing
{
    public class ProbeViewModel : MachineElementViewModel
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public virtual ProbeType ProbeType => ProbeType.None;

        public void DetachFromParent() => Parent.Children.Remove(this);
        
    }
}
