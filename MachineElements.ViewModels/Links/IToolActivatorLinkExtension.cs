using System;

namespace MachineElements.ViewModels.Links
{
    public interface IToolActivatorLinkExtension
    {
        bool ToolActivator { get; }
        void RegisterToolActivation(Action<bool> action);
    }
}
