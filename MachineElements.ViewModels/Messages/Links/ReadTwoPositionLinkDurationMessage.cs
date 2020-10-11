using System;

namespace MachineElements.ViewModels.Messages.Links
{
    public class ReadTwoPositionLinkDurationMessage
    {
        public int LinkId { get; set; }
        public bool RequestedState { get; set; }
        public Action<double> SetDuration { get; set; }
    }
}
