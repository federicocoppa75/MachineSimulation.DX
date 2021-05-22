using System;
using System.Collections.Generic;
using System.Text;

namespace MachineSteps.ViewModels.Messages
{
    public class WaitForChannelFreeMessage
    {
        public int Channel { get; set; }
        public int BackNotifyId { get; set; }
    }
}
