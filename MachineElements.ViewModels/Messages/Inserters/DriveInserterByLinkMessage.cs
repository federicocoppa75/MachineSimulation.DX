namespace MachineElements.ViewModels.Messages.Inserters
{
    public class DriveInserterByLinkMessage
    {
        public int InserterId { get; set; }
        public int LinkId { get; set; }
        public bool Value { get; set; }
    }
}
