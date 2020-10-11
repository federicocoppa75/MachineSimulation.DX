using MachineElements.ViewModels.Messages.Generic;

namespace MachineElements.ViewModels.Messages.ToolChange
{
    public class UnloadToolMessage : BaseBackNotificationIdMessage
    {
        public int ToolSource { get; set; }
        public int ToolSink { get; set; }
    }
}
