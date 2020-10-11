namespace MachineElements.ViewModels.Messages.Links
{
    public class LinearInterpolationLinkMessage : MoveLinearLinkMessage
    {
        public int GroupId { get; private set; }

        public LinearInterpolationLinkMessage(int groupId, int id, double value, double duration) : base(id, value, duration)
        {
            GroupId = groupId;
        }
    }
}
