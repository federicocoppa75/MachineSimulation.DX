using System;
using System.Collections.Generic;
using System.Text;

namespace MachineElements.ViewModels.Messages.Probe
{
    public class CanExecuteRemoveProbeMessage
    {
        public Action<bool> SetValue { get; set; }
    }
}
