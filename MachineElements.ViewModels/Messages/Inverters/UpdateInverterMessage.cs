using MachineElements.ViewModels.Messages.Generic;

namespace MachineElements.ViewModels.Messages.Inverters
{
    public class UpdateInverterMessage : BaseBackNotificationIdMessage
    {
        public int RotationSpeed { get; set; }
        public double Duration { get; set; }
    }
}
