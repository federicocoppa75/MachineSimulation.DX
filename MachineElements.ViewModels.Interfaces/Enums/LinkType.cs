using System.ComponentModel;

namespace MachineElements.ViewModels.Interfaces.Enums
{
    [DefaultValue(Static)]
    public enum LinkType
    {
        Static,

        LinearPosition,

        LinearPneumatic,

        RotaryPneumatic
    }
}
