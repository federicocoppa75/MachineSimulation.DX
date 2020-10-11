using MachineElements.ViewModels.Interfaces.Enums;

namespace MachineElements.ViewModels.Interfaces.Links
{
    public interface ILinkViewModel
    {
        int Id { get; set; }
        LinkDirection Direction { get; set; }
        LinkType LinkType { get; }
        string Description { get; set; }
    }
}
