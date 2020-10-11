using MachineElements.ViewModels.Interfaces.Links;

namespace MachineElements.ViewModels.Links
{
    public interface ILinearLinkGantryMaster
    {
        void SetGantrySlave(IUpdatableValueLink<double> slave);
        void ResetGantry();
    }
}
