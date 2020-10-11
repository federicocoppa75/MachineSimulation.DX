using GalaSoft.MvvmLight.Messaging;
using MachineElements.ViewModels.Messages.Links;
using MachineSteps.Models.Actions;

namespace MachineSteps.ViewModels.Models
{
    public class ArcInterpolatedPositionLinkLazyAction : ArcInterpolatedPositionLinkAction, ILazyAction
    {
        public bool IsUpdated { get; private set; }

        public void Update()
        {
            foreach (var item in Components)
            {
                var i = item;
                Messenger.Default.Send(new ReadLinearLinkStateMessage(i.LinkId, (v) => i.TargetCoordinate = v));
            }

            IsUpdated = true;
        }
    }
}
