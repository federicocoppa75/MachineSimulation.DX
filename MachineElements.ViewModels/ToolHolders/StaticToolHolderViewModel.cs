using MachineElements.ViewModels.Enums;

namespace MachineElements.ViewModels.ToolHolder
{
    public class StaticToolHolderViewModel : ToolHolderViewModel
    {
        public override ToolHolderType ToolHolderType => ToolHolderType.Static;

        public StaticToolHolderViewModel() : base()
        {
        }
    }
}
