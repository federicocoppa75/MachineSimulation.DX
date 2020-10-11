namespace MachineSteps.ViewModels.Models
{
    public interface ILazyAction
    {
        bool IsUpdated { get; }

        void Update();
    }
}
