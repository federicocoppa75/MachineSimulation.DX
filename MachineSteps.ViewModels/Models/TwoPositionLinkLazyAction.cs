using GalaSoft.MvvmLight.Messaging;
using MachineElements.ViewModels.Messages.Links;
using MachineSteps.Models.Actions;

namespace MachineSteps.ViewModels.Models
{
    public class TwoPositionLinkLazyAction : TwoPositionLinkAction, ILazyAction
    {
        public bool IsUpdated { get; private set; }

        public void Update()
        {
            Messenger.Default.Send(new ReadTwoPositionLinkStateMessage(LinkId, (v) => Update(v)));
        }

        private void Update(bool value)
        {
            RequestedState = value ? MachineSteps.Models.Enums.TwoPositionLinkActionRequestedState.On : MachineSteps.Models.Enums.TwoPositionLinkActionRequestedState.Off;
            IsUpdated = true;
        }
    }
}
