using System;

namespace MachineElements.ViewModels.Messages.ToolChange
{
    public class GetAvailableToolSinkMessage
    {
        public Action<int, string> SetAvailableToolSink { get; set; }
    }
}
