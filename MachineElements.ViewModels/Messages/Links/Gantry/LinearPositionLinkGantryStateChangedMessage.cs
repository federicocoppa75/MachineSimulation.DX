using System;
using System.Collections.Generic;
using System.Text;

namespace MachineElements.ViewModels.Messages.Links.Gantry
{
    public class LinearPositionLinkGantryStateChangedMessage
    {
        public int Id { get; set; }
        public bool IsSlave { get; set; }
    }
}
