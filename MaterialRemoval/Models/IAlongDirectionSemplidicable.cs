using MaterialRemoval.Enums;

namespace MaterialRemoval.Models
{
    public interface IAlongDirectionSemplidicable
    {
        AlongDirectionSemplificationCheckResult Check(ImplicitToolBase tool);
    }
}
