using System;

namespace MachineElements.ViewModels.Messages.Links
{
    public class ReadLinkLimitsMessage
    {
        public int LinkId { get; set; }
        public Action<double, double> SetLimits { get; set; }
    }
}
