using MachineElements.ViewModels.Interfaces.Links;
using System;
using System.Collections.ObjectModel;

namespace MachineElements.ViewModels.Interfaces
{
    public interface IStepExecutionInfoProvider
    {
        bool IsStepTimeVisible { get; }
        bool IsAxesStateVisible { get; }
        ObservableCollection<IUpdatableValueLink<double>> LinearPositionLinks { get; }
        TimeSpan StepTime { get; }
        string InverterName { get; }
        int InverterValue { get; }
        bool IsInverterStateVisible { get; }
    }
}
