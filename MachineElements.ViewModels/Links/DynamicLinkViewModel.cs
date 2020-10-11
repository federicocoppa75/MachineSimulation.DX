using GalaSoft.MvvmLight;
using MachineElements.ViewModels.Interfaces.Enums;
using MachineElements.ViewModels.Interfaces.Links;

namespace MachineElements.ViewModels.Links
{
    public abstract class DynamicLinkViewModel : ViewModelBase, ILinkViewModel
    {
        public int Id { get; set; }
        public LinkDirection Direction { get; set; }
        public abstract LinkType LinkType { get; }
        public string Description { get; set; }
    }
}
