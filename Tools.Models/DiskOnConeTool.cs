using Tools.Models.Enums;

namespace Tools.Models
{
    public class DiskOnConeTool : DiskTool
    {
        public double PostponemntLength { get; set; }

        public double PostponemntDiameter { get; set; }

        public override ToolType ToolType => ToolType.DiskOnCone;

        public override double GetTotalLength()
        {
            var bt = BodyThickness;
            var tl = base.GetTotalLength();

            return PostponemntLength + bt / 2.0 + tl / 2.0;
        }
    }
}
