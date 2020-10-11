using System;
using System.Collections.Generic;
using System.Text;

namespace MachineElements.ViewModels.Interfaces.Messages
{
    public class GetMachineViewModel
    {
        public Action<IMachineViewModel> SetAction { get; set; }
    }
}
