using GalaSoft.MvvmLight.Messaging;
using MachineElements.ViewModels.Messages.Links;
using MachineSteps.Models.Actions;

namespace MachineSteps.ViewModels.Models
{
    public class LinearInterpolatedPositionLinkLazyAction : LinearInterpolatedPositionLinkAction, ILazyAction
    {
        public bool IsUpdated { get; private set; }

        public void Update()
        {
            foreach (var pos in Positions)
            {
                var p = pos;
                Messenger.Default.Send(new ReadLinearLinkStateMessage(p.LinkId, (v) => p.RequestPosition = v));
            }

            IsUpdated = true;
        }
    }
}
