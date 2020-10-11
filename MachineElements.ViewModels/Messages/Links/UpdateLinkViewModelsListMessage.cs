using MachineElements.ViewModels.Interfaces.Links;
using System.Collections.Generic;

namespace MachineElements.ViewModels.Messages.Links
{
    public class UpdateLinkViewModelsListMessage
    {
        public IEnumerable<ILinkViewModel> LinkViewModels { get; set; }
    }
}
