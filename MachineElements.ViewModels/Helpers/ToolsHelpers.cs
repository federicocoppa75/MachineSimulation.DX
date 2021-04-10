using HelixToolkit.Wpf.SharpDX;
using System;
using System.Linq;
using Tools.Models;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace MachineElements.ViewModels.Helpers
{
    internal static class ToolsHelpers
    {
        public static Geometry3D GetConeModel(string coneFile)
        {
            Geometry3D geometry = null;
            var reader = new StLReader();
            var objList = reader.Read(coneFile);

            if (objList?.Count > 0)
            {
                geometry = objList[0].Geometry;
                geometry.UpdateOctree();
            }

            return geometry;
        }

        public static Geometry3D GetToolModel(Tool tool, Point3D position, Vector3D direction)
        {
            Geometry3D model = null;

            switch (tool.ToolType)
            {
                case global::Tools.Models.Enums.ToolType.None:
                    break;
                case global::Tools.Models.Enums.ToolType.Base:
                    break;
                case global::Tools.Models.Enums.ToolType.Simple:
                    model = GetSimpleModel(tool, position, direction);
                    break;
                case global::Tools.Models.Enums.ToolType.TwoSection:
                    model = GetTwoSectionModel(tool, position, direction);
                    break;
                case global::Tools.Models.Enums.ToolType.Pointed:
                    model = GetPointedModel(tool, position, direction);
                    break;
                case global::Tools.Models.Enums.ToolType.Disk:
                    model = GetDiskModel(tool, position, direction);
                    break;
                case global::Tools.Models.Enums.ToolType.BullNoseConcave:
                    break;
                case global::Tools.Models.Enums.ToolType.BullNoseConvex:
                    break;
                case global::Tools.Models.Enums.ToolType.Composed:
                    break;
                case global::Tools.Models.Enums.ToolType.Countersink:
                    model = GetCountersinkModel(tool, position, direction);
                    break;
                case global::Tools.Models.Enums.ToolType.DiskOnCone:
                    model = GetDiskOnConeModel(tool, position, direction);
                    break;
                default:
                    break;
            }

            if (model == null) throw new NotImplementedException();

            return model;
        }

        private static Geometry3D GetSimpleModel(Tool tool, Point3D position, Vector3D direction)
        {
            var t = tool as SimpleTool;
            var builder = new MeshBuilder();
            var p = position + direction * t.Length;

            builder.AddCylinder(position.ToVector3(),
                                p.ToVector3(),
                                t.Diameter / 2.0);

            return builder.ToMesh();
        }

        private static Geometry3D GetTwoSectionModel(Tool tool, Point3D position, Vector3D direction)
        {
            var t = tool as TwoSectionTool;
            var builder = new MeshBuilder();
            var p1 = position + direction * t.Length1;
            var p2 = position + direction * (t.Length1 + t.Length2);

            builder.AddCylinder(position.ToVector3(),
                                p1.ToVector3(),
                                t.Diameter1 / 2.0);
            builder.AddCylinder(p1.ToVector3(),
                                p2.ToVector3(),
                                t.Diameter2 / 2.0);

            return builder.ToMesh();
        }

        private static Geometry3D GetPointedModel(Tool tool, Point3D position, Vector3D direction)
        {
            var t = tool as PointedTool;
            var builder = new MeshBuilder();
            var p1 = position + direction * t.StraightLength;
            var p2 = position + direction * (t.StraightLength + t.ConeHeight);

            builder.AddCylinder(position.ToVector3(),
                                p1.ToVector3(),
                                t.Diameter / 2.0);
            builder.AddCone(p1.ToVector3(),
                            p2.ToVector3(),
                            t.Diameter / 2.0,
                            false,
                            20);

            return builder.ToMesh();
        }

        private static Geometry3D GetDiskModel(Tool tool, Point3D position, Vector3D direction)
        {
            var t = tool as DiskTool;
            var builder = new MeshBuilder();
            var d = Math.Abs(t.BodyThickness - t.CuttingThickness) / 2.0;
            var r1 = t.Diameter / 2.0 - t.CuttingRadialThickness;
            var profile = new[]
            {
                new SharpDX.Vector2(0.0f, 10.0f),
                new SharpDX.Vector2(0.0f, (float)r1),
                new SharpDX.Vector2((float)(- d), (float)r1),
                new SharpDX.Vector2((float)(- d), (float)(t.Diameter / 2.0)),
                new SharpDX.Vector2((float)(t.BodyThickness + d), (float)(t.Diameter / 2.0)),
                new SharpDX.Vector2((float)(t.BodyThickness + d), (float)r1),
                new SharpDX.Vector2((float)t.BodyThickness, (float)r1),
                new SharpDX.Vector2((float)t.BodyThickness, 10.0f)
            };

            builder.AddRevolvedGeometry(profile.ToList(),
                                        null,
                                        position.ToVector3(),
                                        direction.ToVector3(),
                                        100);

            return builder.ToMesh();
        }

        private static Geometry3D GetDiskOnConeModel(Tool tool, Point3D position, Vector3D direction)
        {
            var t = tool as DiskOnConeTool;
            var builder = new MeshBuilder();
            var d = Math.Abs(t.BodyThickness - t.CuttingThickness) / 2.0;
            var r1 = t.Diameter / 2.0 - t.CuttingRadialThickness;
            var p1 = position + direction * t.PostponemntLength;
            var profile = new SharpDX.Vector2[]
            {
                new SharpDX.Vector2(0.0f, (float)(t.PostponemntDiameter / 2.0)),
                new SharpDX.Vector2(0.0f, (float)r1),
                new SharpDX.Vector2((float)(- d), (float)r1),
                new SharpDX.Vector2((float)(- d), (float)(t.Diameter / 2.0)),
                new SharpDX.Vector2((float)(t.BodyThickness + d), (float)(t.Diameter / 2.0)),
                new SharpDX.Vector2((float)(t.BodyThickness + d), (float)r1),
                new SharpDX.Vector2((float)t.BodyThickness, (float)r1),
                new SharpDX.Vector2((float)t.BodyThickness, (float)(t.PostponemntDiameter / 2.0)),

            };

            builder.AddRevolvedGeometry(profile.ToList(),
                                        null,
                                        p1.ToVector3(),
                                        direction.ToVector3(),
                                        100);
            builder.AddCylinder(position.ToVector3(),
                                p1.ToVector3(),
                                t.PostponemntDiameter,
                                20);

            return builder.ToMesh();
        }

        public static Geometry3D GetCountersinkModel(Tool tool, Point3D position, Vector3D direction)
        {
            const double hSvasatore = 10.0;
            var t = tool as CountersinkTool;
            var builder = new MeshBuilder();
            var p1 = position + direction * (t.Length1 - hSvasatore);
            var p12 = position + direction * t.Length1;
            var p2 = position + direction * (t.Length1 + t.Length2);
            var p3 = position + direction * (t.Length1 + t.Length2 + t.Length3);

            builder.AddCylinder(position.ToVector3(),
                                p1.ToVector3(),
                                t.Diameter1 / 2.0);
            builder.AddCylinder(p1.ToVector3(),
                                p12.ToVector3(),
                                t.Diameter2 / 2.0);
            builder.AddCone(p12.ToVector3(),
                            direction.ToVector3(),
                            t.Diameter2 / 2.0, 
                            t.Diameter1 / 2.0, 
                            t.Length2, 
                            false, 
                            false, 
                            20);
            builder.AddCylinder(p2.ToVector3(),
                                p3.ToVector3(), 
                                t.Diameter1 / 2.0);

            return builder.ToMesh();
        }

        public static Point3D ToPoint3D(this Vector v) => new Point3D(v.X, v.Y, v.Z);

        public static Vector3D ToVector3D(this Vector v) => new Vector3D(v.X, v.Y, v.Z);
    }
}
