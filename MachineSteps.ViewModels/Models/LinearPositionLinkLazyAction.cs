using GalaSoft.MvvmLight.Messaging;
using MachineElements.ViewModels.Messages.Links;
using MachineSteps.Models.Actions;

namespace MachineSteps.ViewModels.Models
{
    public class LinearPositionLinkLazyAction : LinearPositionLinkAction, ILazyAction
    {
        public bool IsUpdated { get; private set; }

        public void Update()
        {
            Messenger.Default.Send(new ReadLinearLinkStateMessage(LinkId, (v) => RequestedPosition = v));
            IsUpdated = true;
        }
    }
}
