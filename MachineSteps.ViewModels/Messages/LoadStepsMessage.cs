using MachineSteps.Models.Steps;
using System.Collections.Generic;

namespace MachineSteps.ViewModels.Messages
{
    public class LoadStepsMessage
    {
        public List<MachineStep> Steps { get; set; }
    }
}
