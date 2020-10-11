using System;

namespace MachineElements.ViewModels.Messages.ToolChange
{
    public class GetAvailableToolMessage
    {
        public Action<int, string> SetAvailableTool { get; set; }
    }
}
