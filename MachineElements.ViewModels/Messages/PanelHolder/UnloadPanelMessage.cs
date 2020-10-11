using MachineElements.ViewModels.Messages.Generic;
using System;

namespace MachineElements.ViewModels.Messages.PanelHolder
{
    public class UnloadPanelMessage : BaseBackNotificationIdMessage
    {
        public int PanelHolderId { get; set; }
        public Action<bool> NotifyExecution { get; set; }
    }
}
