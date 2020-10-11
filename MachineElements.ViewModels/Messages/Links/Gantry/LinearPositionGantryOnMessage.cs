namespace MachineElements.ViewModels.Messages.Links.Gantry
{
    public class LinearPositionGantryOnMessage  : LinearPositionGantryBaseMessage
    {
        public override void Execute()
        {
            if (!UnhookedSlave) Master.SetGantrySlave(Slave);
        }
    }
}
