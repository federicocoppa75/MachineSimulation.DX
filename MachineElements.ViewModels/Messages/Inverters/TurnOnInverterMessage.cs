using MachineElements.ViewModels.Messages.Generic;

namespace MachineElements.ViewModels.Messages.Inverters
{
    public class TurnOnInverterMessage : BaseBackNotificationIdMessage
    {
        public int Head { get; set; }
        public int Order { get; set; }
        public int RotationSpeed { get; set; }
        public double Duration { get; set; }
    }
}
