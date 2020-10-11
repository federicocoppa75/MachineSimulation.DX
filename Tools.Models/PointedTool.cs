using System;
using Tools.Models.Enums;

namespace Tools.Models
{
    public class PointedTool : Tool
    {
        public double Diameter { get; set; }

        public double StraightLength { get; set; }

        public double ConeHeight { get; set; }

        public double UsefulLength { get; set; }

        public override double GetTotalDiameter() => Diameter;

        public override double GetTotalLength() => StraightLength + ConeHeight;

        public override ToolType ToolType => ToolType.Pointed;
    }
}
