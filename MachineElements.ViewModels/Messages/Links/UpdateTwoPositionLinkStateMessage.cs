namespace MachineElements.ViewModels.Messages.Links
{
    public class UpdateTwoPositionLinkStateMessage : UpdateLinkStateMessage<bool>
    {
        public int GroupId { get; set; }

        public UpdateTwoPositionLinkStateMessage(int id, bool value, int groupId = -1) : base(id, value)
        {
            GroupId = groupId;
        }
    }
}
