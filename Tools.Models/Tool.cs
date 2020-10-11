using Tools.Models.Enums;

namespace Tools.Models
{
    public abstract class Tool
    {
        public string Name { get; set; }

        public string Description { get; set; }        

        public abstract double GetTotalDiameter();

        public abstract double GetTotalLength();

        public virtual ToolType ToolType => ToolType.Base;

        public ToolLinkType ToolLinkType { get; set; }

        public string ConeModelFile { get; set; }
    }
}
