using MachineElements.Models.Panel;
using System;

namespace MachineElements.ViewModels.Messages.PanelHolder
{
    public class GetPanelDataMessage
    {
        public Action<PanelData> SetPanelData { get; set; }
    }
}
