using System;
using System.Collections.Generic;
using System.Text;

namespace MachineElements.ViewModels.Interfaces.Messages.Panel
{
    public class PanelPresenceRequestMessage
    {
        public Action Confirm { get; set; }
    }
}
