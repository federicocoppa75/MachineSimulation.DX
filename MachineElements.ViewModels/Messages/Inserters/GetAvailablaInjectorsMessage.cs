using System;

namespace MachineElements.ViewModels.Messages.Inserters
{
    public class GetAvailablaInjectorsMessage
    {
        public Action<int> SetInjectorData { get; set; }
    }
}
