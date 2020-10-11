using System;
using System.Collections.Generic;
using System.Text;

namespace MachineElements.ViewModels.Messages.Links.Gantry
{
    public class LinearPositionGantryOffMessage : LinearPositionGantryBaseMessage
    {
        public override void Execute()
        {
            Master.ResetGantry();
        }
    }
}
