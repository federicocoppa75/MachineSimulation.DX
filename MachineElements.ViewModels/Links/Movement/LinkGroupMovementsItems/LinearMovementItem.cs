namespace MachineElements.ViewModels.Links.Movement.LinkGroupMovementsItems
{
    public class LinearMovementItem : MovementItem
    {
        public double Value { get; private set; }

        public LinearMovementItem(int linkId, double value, double targetValue) : base(linkId, targetValue)
        {
            Value = value;
        }

        public override void SetValue(double k) => ActualValue = (TargetValue - Value) * k + Value;
    }
}
