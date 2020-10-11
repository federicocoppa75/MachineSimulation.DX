namespace MachineElements.ViewModels.Interfaces.Links
{
    public interface IUpdatableValueLink
    {
        int Id { get; }
    }

    public interface IUpdatableValueLink<T> : IUpdatableValueLink where T : struct
    {
        T Value { get; set; }
    }
}
