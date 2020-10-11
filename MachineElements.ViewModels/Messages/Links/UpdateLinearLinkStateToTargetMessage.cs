namespace MachineElements.ViewModels.Messages.Links
{
    public class UpdateLinearLinkStateToTargetMessage : UpdateLinearLinkStateMessage
    {
        public bool IsCompleted { get; private set; }

        public UpdateLinearLinkStateToTargetMessage(int id, double value, bool isCompleted = false) : base(id, value)
        {
            IsCompleted = isCompleted;
        }
    }
}
