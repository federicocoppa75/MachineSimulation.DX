using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tooling.Models.IO.Extensions;
using ExTooling = MachineModels.Models.Tooling;
using ExTools = MachineModels.Models.Tools;
using IntTooling = Tooling.Models;
using IntTools = Tools.Models;

namespace Tooling.Models.IO
{
    public static class ToolingLoader
    {
        public static bool LoadTooling(string toolingFile, out IntTooling.Tooling tooling, out IntTools.ToolSet toolSet)
        {
            var result = LoadTooling(toolingFile, out ExTooling.Tooling too, out ExTools.ToolSet ts);

            tooling = too.ToInternal();
            toolSet = ts.ToInternal();

            return result;
        }

        private static bool LoadTooling(string toolingFile, out ExTooling.Tooling tooling, out ExTools.ToolSet toolSet)
        {
            toolSet = null;
            tooling = null;
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ExTooling.Tooling));

            using (var reader = new System.IO.StreamReader(toolingFile))
            {
                tooling = (ExTooling.Tooling)serializer.Deserialize(reader);
            }

            if(tooling != null)
            {
                toolSet = GetToolSet(tooling.ToolsFile);
            }

            return (toolSet != null) && (tooling != null);
        }

        private static ExTools.ToolSet GetToolSet(string toolsFile)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ExTools.ToolSet));

            using (var reader = new System.IO.StreamReader(toolsFile))
            {
                return (ExTools.ToolSet)serializer.Deserialize(reader);
            }
        }
    }
}
