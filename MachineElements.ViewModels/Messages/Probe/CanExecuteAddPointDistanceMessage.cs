using System;
using System.Collections.Generic;
using System.Text;

namespace MachineElements.ViewModels.Messages.Probe
{
    public class CanExecuteAddPointDistanceMessage
    {
        public Action<bool> SetValue { get; set; }
    }
}
