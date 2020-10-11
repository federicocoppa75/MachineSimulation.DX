using System;
using System.Collections.Generic;
using System.Text;

namespace MachineElements.ViewModels.Interfaces.Messages
{
    public class SelectMachineElementMessage
    {
        public IMachineElementViewModel Element { get; set; }
    }
}
