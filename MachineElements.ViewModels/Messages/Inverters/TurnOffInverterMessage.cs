using MachineElements.ViewModels.Messages.Generic;

namespace MachineElements.ViewModels.Messages.Inverters
{
    public class TurnOffInverterMessage : BaseBackNotificationIdMessage
    {
        public int Head { get; set; }
        public int Order { get; set; }
        public double Duration { get; set; }
    }
}
