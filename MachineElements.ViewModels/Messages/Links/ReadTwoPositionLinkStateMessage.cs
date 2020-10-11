using System;

namespace MachineElements.ViewModels.Messages.Links
{
    public class ReadTwoPositionLinkStateMessage : ReadLinkStateMessage<bool>
    {
        public ReadTwoPositionLinkStateMessage(int id, Action<bool> setValue) : base(id, setValue)
        {
        }
    }
}
