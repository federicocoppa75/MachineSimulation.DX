using MachineElements.ViewModels.Interfaces.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MachineElements.ViewModels.Interfaces.Steps
{
    public interface IStepObserver
    {
        int Index { get; }
        StepExecutionDirection Direction { get; }

        void SetFarwardIndex(int index);
        void SetBackIndex(int index);
    }
}
