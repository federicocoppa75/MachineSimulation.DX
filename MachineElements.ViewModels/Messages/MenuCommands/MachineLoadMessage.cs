using MachineElements.ViewModels.Interfaces;
using System.Collections.Generic;

namespace MachineElements.ViewModels.Messages.MenuCommands
{
    public class MachineLoadMessage
    {
        public IList<IMachineElementViewModel> Machine { get; set; }
    }
}
