using System.Collections.Generic;

namespace Tooling.Models
{
    public class Tooling
    {
        public string MachineFile { get; set; }
        public string ToolsFile { get; set; }
        public List<ToolingUnit> Units { get; set; }
    }
}
