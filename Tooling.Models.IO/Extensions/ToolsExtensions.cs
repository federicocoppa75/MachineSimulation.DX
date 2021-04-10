using System;
using ExTools = MachineModels.Models.Tools;
using IntTools = Tools.Models;
using MMM = MachineModels.Models;
using System.Linq;
using System.Collections.Generic;

namespace Tooling.Models.IO.Extensions
{
    internal static class ToolsExtensions
    {
        public static IntTools.ToolSet ToInternal(this ExTools.ToolSet toolSet)
        {
            var tSet = new IntTools.ToolSet() 
            { 
                Tools = new System.Collections.Generic.List<IntTools.Tool>()
            };

            foreach (var item in toolSet.Tools)
            {
                if(item.ToolType != MachineModels.Enums.ToolType.AngularTransmission)
                {
                    tSet.Tools.Add(ToInternal(item));
                }                
            }

            var tDictionary = tSet.Tools.ToDictionary(t => t.Name, t => t);

            foreach (var item in toolSet.Tools)
            {
                if (item.ToolType == MachineModels.Enums.ToolType.AngularTransmission)
                {
                    tSet.Tools.Add(ToInternal(item as ExTools.AngolarTransmission, tDictionary));
                }
            }

            return tSet;
        }

        private static IntTools.Tool ToInternal(ExTools.Tool item)
        {
            IntTools.Tool t = null;

            switch (item.ToolType)
            {

                case MachineModels.Enums.ToolType.Simple:
                    t = ToInternal((ExTools.SimpleTool)item);
                    break;
                case MachineModels.Enums.ToolType.TwoSection:
                    t = ToInternal((ExTools.TwoSectionTool)item);
                    break;
                case MachineModels.Enums.ToolType.Pointed:
                    t = ToInternal((ExTools.PointedTool)item);
                    break;
                case MachineModels.Enums.ToolType.Disk:
                    t = ToInternal((ExTools.DiskTool)item);
                    break;
                case MachineModels.Enums.ToolType.Countersink:
                    t = ToInternal((ExTools.CountersinkTool)item);
                    break;
                case MachineModels.Enums.ToolType.DiskOnCone:
                    t = ToInternal((ExTools.DiskOnConeTool)item);
                    break;
                //case MachineModels.Enums.ToolType.Base:
                //case MachineModels.Enums.ToolType.None:
                //case MachineModels.Enums.ToolType.BullNoseConcave:
                //case MachineModels.Enums.ToolType.BullNoseConvex:
                //case MachineModels.Enums.ToolType.Composed:
                default:
                    throw new NotImplementedException();
            }

            return t;
        }

        private static IntTools.Tool ToInternal(ExTools.CountersinkTool item)
        {
            return new IntTools.CountersinkTool()
            {
                Name = item.Name,
                Description = item.Description,
                ConeModelFile = item.ConeModelFile,
                Diameter1 = item.Diameter1,
                Length1 = item.Length1,
                Diameter2 = item.Diameter2,
                Length2 = item.Length2,
                Length3 = item.Length3
            };
        }

        private static IntTools.Tool ToInternal(ExTools.DiskOnConeTool item)
        {
            return new IntTools.DiskOnConeTool()
            {
                Name = item.Name,
                Description = item.Description,
                ConeModelFile = item.ConeModelFile,
                Diameter = item.Diameter,
                CuttingRadialThickness = item.CuttingRadialThickness,
                BodyThickness = item.BodyThickness,
                CuttingThickness = item.CuttingThickness,
                RadialUsefulLength = item.RadialUsefulLength,
                PostponemntLength = item.PostponemntLength,
                PostponemntDiameter = item.PostponemntDiameter
            };
        }

        private static IntTools.Tool ToInternal(ExTools.DiskTool item)
        {
            return new IntTools.DiskTool()
            {
                Name = item.Name,
                Description = item.Description,
                ConeModelFile = item.ConeModelFile,
                Diameter = item.Diameter,
                CuttingRadialThickness = item.CuttingRadialThickness,
                BodyThickness = item.BodyThickness,
                CuttingThickness = item.CuttingThickness,
                RadialUsefulLength = item.RadialUsefulLength
            };
        }

        private static IntTools.Tool ToInternal(ExTools.PointedTool item)
        {
            return new IntTools.PointedTool()
            {
                Name = item.Name,
                Description = item.Description,
                ConeModelFile = item.ConeModelFile,
                Diameter = item.Diameter,
                StraightLength = item.StraightLength,
                ConeHeight = item.ConeHeight,
                UsefulLength = item.UsefulLength
            };
        }

        private static IntTools.Tool ToInternal(ExTools.SimpleTool item)
        {
            return new IntTools.SimpleTool()
            {
                Name = item.Name,
                Description = item.Description,
                ConeModelFile = item.ConeModelFile,
                Diameter = item.Diameter,
                Length = item.Length,
                UsefulLength = item.UsefulLength
            };
        }

        private static IntTools.Tool ToInternal(ExTools.TwoSectionTool item)
        {
            return new IntTools.TwoSectionTool()
            {
                Name = item.Name,
                Description = item.Description,
                ConeModelFile = item.ConeModelFile,
                Diameter1 = item.Diameter1,
                Length1 = item.Length1,
                Diameter2 = item.Diameter2,
                Length2 = item.Length2,
                UsefulLength = item.UsefulLength
            };
        }

        private static IntTools.Tool ToInternal(ExTools.AngolarTransmission angolarTransmission, Dictionary<string, IntTools.Tool> tools)
        {
            var at = new IntTools.AngolarTransmission() 
            {
                Name = angolarTransmission.Name,
                Description = angolarTransmission.Description,
                ConeModelFile = angolarTransmission.ConeModelFile,
                BodyModelFile = angolarTransmission.BodyModelFile 
            };

            foreach (var item in angolarTransmission.Subspindles)
            {
                at.Subspindles.Add(new IntTools.AngolarTransmission.Subspindle()
                {
                    Position = item.Position.ToInternal(),
                    Direction = item.Direction.ToInternal(),
                    Tool = tools[item.ToolName]
                });
            }

            return at;
        }

        public static IntTools.Vector ToInternal(this MMM.Vector v) => new IntTools.Vector() { X = v.X, Y = v.Y, Z = v.Z };
    }
}
