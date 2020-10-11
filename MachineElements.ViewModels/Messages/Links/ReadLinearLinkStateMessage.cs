using System;

namespace MachineElements.ViewModels.Messages.Links
{
    public class ReadLinearLinkStateMessage : ReadLinkStateMessage<double>
    {
        public ReadLinearLinkStateMessage(int id, Action<double> setValue) : base(id, setValue)
        {
        }
    }
}
