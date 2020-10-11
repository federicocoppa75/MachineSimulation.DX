using System;

namespace MachineElements.ViewModels.Links.Movement.LinkGroupMovementsItems
{
    public class YArcMovementItem : ArcMovementItem
    {
        public YArcMovementItem(int linkId, double targetValue) : base(linkId, targetValue)
        {
        }

        public override void SetValue(double k)
        {
            double a = Normalize(StartAngle + Angle * k);

            ActualValue = CenterCoordinate + Math.Sin(a) * Radius;
        }
    }
}
