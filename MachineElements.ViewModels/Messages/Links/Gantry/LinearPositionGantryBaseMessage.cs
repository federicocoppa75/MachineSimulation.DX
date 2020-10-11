using MachineElements.ViewModels.Interfaces.Links;
using MachineElements.ViewModels.Links;
using MachineElements.ViewModels.Messages.Generic;

namespace MachineElements.ViewModels.Messages.Links.Gantry
{
    public abstract class LinearPositionGantryBaseMessage : BaseBackNotificationIdMessage
    {
        public int MasterId { get; set; }
        public int SlaveId { get; set; }
        public ILinearLinkGantryMaster Master { get; set; }
        public IUpdatableValueLink<double> Slave { get; set; }
        public bool IsReady => (Master != null) && ((Slave != null) || VirtualSlave || UnhookedSlave);
        public bool VirtualSlave { get; set; }
        public bool UnhookedSlave { get; set; }

        public abstract void Execute();
    }
}
