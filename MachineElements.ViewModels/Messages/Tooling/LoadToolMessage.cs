using Tools.Models;

namespace MachineElements.ViewModels.Messages.Tooling
{
    public class LoadToolMessage
    {
        public int ToolHolderId { get; set; }
        public Tool Tool { get; set; }
    }
}
