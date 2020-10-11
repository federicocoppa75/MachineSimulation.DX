using ExTooling = MachineModels.Models.Tooling;
using IntTooling = Tooling.Models;

namespace Tooling.Models.IO.Extensions
{
    internal static class ToolingExtensions
    {
        public static IntTooling.Tooling ToInternal(this ExTooling.Tooling tooling)
        {
            var t = new IntTooling.Tooling()
            {
                MachineFile = tooling.MachineFile,
                ToolsFile = tooling.ToolsFile,
                Units = new System.Collections.Generic.List<IntTooling.ToolingUnit>()
            };

            foreach (var item in tooling.Units)
            {
                t.Units.Add(new IntTooling.ToolingUnit()
                {
                    ToolName = item.ToolName,
                    ToolHolderId = item.ToolHolderId
                });
            }

            return t;
        }
    }
}
