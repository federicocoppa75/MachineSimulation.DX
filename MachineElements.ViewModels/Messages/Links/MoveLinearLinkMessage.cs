namespace MachineElements.ViewModels.Messages.Links
{
    public class MoveLinearLinkMessage : UpdateLinkStateMessage<double>
    {
        public double Duration { get; private set; }

        public MoveLinearLinkMessage(int id, double value, double duration) : base(id, value)
        {
            Duration = duration;
        }
    }
}
