using MachineElements.ViewModels.Messages.Generic;

namespace MachineElements.ViewModels.Messages.Inserters
{
    public class InjectActionMessage : BaseBackNotificationIdMessage
    {
        public int InjectorId { get; set; }

        public double Duration { get; set; }
    }
}
