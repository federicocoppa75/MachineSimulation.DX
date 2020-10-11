using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Messages.Generic;
using Tools.Models;

namespace MachineElements.ViewModels.Messages.ToolChange
{
    public class CompleteToolLoadingMessage : BaseBackNotificationIdMessage
    {
        public int ToolSink { get; set; }
        public IMachineElementViewModel ToolModel { get; set; }
        
        public Tool ToolData { get; set; }
    }
}
