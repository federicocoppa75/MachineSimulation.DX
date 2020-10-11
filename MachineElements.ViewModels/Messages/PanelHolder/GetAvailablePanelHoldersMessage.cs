using MachineElements.ViewModels.Enums;
using System;

namespace MachineElements.ViewModels.Messages.PanelHolder
{
    public class GetAvailablePanelHoldersMessage
    {
        public Action<int, string, PanelLoadType> AvailableToolHolder { get; set; }
    }
}
