using System;
using Tools.Models.Enums;

namespace Tools.Models
{
    public class SimpleTool : Tool
    {
        public double Diameter { get; set; }

        public double Length { get; set; }

        public double UsefulLength { get; set; }

        public override double GetTotalDiameter() => Diameter;

        public override double GetTotalLength() => Length;

        public override ToolType ToolType => ToolType.Simple;
    }
}
