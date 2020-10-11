using MachineElements.ViewModels.Messages.Generic;

namespace MachineElements.ViewModels.Messages.ToolChange
{
    public class LoadToolMessage : BaseBackNotificationIdMessage
    {
        public int ToolSource { get; set; }
        public int ToolSink { get; set; }
    }
}
