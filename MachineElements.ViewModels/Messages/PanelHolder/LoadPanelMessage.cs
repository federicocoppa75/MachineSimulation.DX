using MachineElements.ViewModels.Messages.Generic;
using System;

namespace MachineElements.ViewModels.Messages.PanelHolder
{
    public class LoadPanelMessage : BaseBackNotificationIdMessage
    {
        public int PanelHolderId { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Action<bool> NotifyExecution { get; set; }
    }
}
