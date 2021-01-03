using g3;
using MaterialRemoval.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;
using Point3DCollection = System.Windows.Media.Media3D.Point3DCollection;
using Vector3Collection = HelixToolkit.Wpf.SharpDX.Vector3Collection;
using HelixToolkit.Wpf.SharpDX;

namespace MaterialRemoval.Extensions
{
    public static class GeometryExtension
    {
        public static Vector3d ToVector3d(this Vector3D vector) => new Vector3d(vector.X, vector.Y, vector.Z);

        public static Vector3d ToVector3d(this Point3D point) => new Vector3d(point.X, point.Y, point.Z);

        public static Point3D ToPoint3D(this Vector3d vector) => new Point3D(vector.x, vector.y, vector.z);

        public static Vector3d ToVector3d(this SharpDX.Vector3 vector) => new Vector3d(vector.X, vector.Y, vector.Z);

        public static Vector3Collection ToVector3Collection(this Point3DCollection coll)
        {
            var outColl = new Vector3Collection(coll.Count);

            for (int i = 0; i < coll.Count; i++) outColl.Add(coll[i].ToVector3());

            return outColl;
        }

        public static Vector3Collection ToVector3Collection(this List<Point3D> coll)
        {
            var outColl = new Vector3Collection(coll.Count);

            for (int i = 0; i < coll.Count; i++) outColl.Add(coll[i].ToVector3());

            return outColl;
        }


        public static Vector3Collection ToVector3Collection(this List<Vector3D> coll)
        {
            var outColl = new Vector3Collection(coll.Count);

            for (int i = 0; i < coll.Count; i++) outColl.Add(coll[i].ToVector3());

            return outColl;
        }

        public static MeshGeometry3D ToMeshGeometry3D(this DMesh3 mesh)
        {
            var vertices = mesh.Vertices();
            var uiPoints = new Point3DCollection(mesh.VertexCount);
            var indexes = mesh.TriangleIndices();
            var uiIndexes = new IntCollection();

            foreach (var v in vertices)
            {
                uiPoints.Add(v.ToPoint3D());
            }

            foreach (var i in indexes)
            {
                uiIndexes.Add(i);
            }

            return new MeshGeometry3D()
            {
                Positions = uiPoints.ToVector3Collection(),
                TriangleIndices = uiIndexes
            };
        }

        public static DMesh3 ToDMesh3(this MeshGeometry3D mesh)
        {
            var dMesh = new DMesh3();
            var positions = mesh.Positions;
            var indexes = mesh.TriangleIndices;

            foreach (var p in positions)
            {
                dMesh.AppendVertex(p.ToVector3d());
            }

            for (int i = 0; i < indexes.Count; i+=3)
            {
                dMesh.AppendTriangle(indexes[i], indexes[i + 1], indexes[i + 2]);
            }

            return dMesh;
        }

        public static void UpdateFrom(this MeshGeometry3D dest, DMesh3 src)
        {
            //if (!src.IsCompact) src.CompactInPlace();

            //var vertices = src.VerticesBuffer;

            //foreach (int vId in src.VerticesRefCounts)
            //{
            //    int i = vId * 3;
            //    dest.Positions.Add(new Point3D(vertices[i], vertices[i + 1], vertices[i + 2]));
            //}

            //var triangles = src.TrianglesBuffer;

            //foreach (int tId in src.TrianglesRefCounts)
            //{
            //    int i = tId * 3;
            //    dest.TriangleIndices.Add(triangles[i]);
            //    dest.TriangleIndices.Add(triangles[i + 1]);
            //    dest.TriangleIndices.Add(triangles[i + 2]);
            //}

            var positions = new List<Point3D>(src.VerticesRefCounts.count);
            var tringleindices = new List<int>(src.TrianglesRefCounts.count);
            var tasks = new Task[2];

            tasks[0] = Task.Run(() =>
            {
                var vertices = src.VerticesBuffer;

                foreach (int vId in src.VerticesRefCounts)
                {
                    int i = vId * 3;
                    positions.Add(new Point3D(vertices[i], vertices[i + 1], vertices[i + 2]));
                }
            });

            tasks[1] = Task.Run(() =>
            {
                var triangles = src.TrianglesBuffer;

                foreach (int tId in src.TrianglesRefCounts)
                {
                    int i = tId * 3;
                    tringleindices.Add(triangles[i]);
                    tringleindices.Add(triangles[i + 1]);
                    tringleindices.Add(triangles[i + 2]);
                }
            });

            Task.WaitAll(tasks);

            dest.Positions = positions.ToVector3Collection();
            dest.TriangleIndices = new IntCollection(tringleindices);
        }

        public static bool SmartAdd(this List<BoundedImplicitFunction3d> list, ImplicitToolBase tool)
        {
            var count = list.Count;
            var result = true;

            if (count > 0)
            {
                var lastIndex = count - 1;
                var last = list[lastIndex];

                if (tool.IsCloseTo(last as ImplicitToolBase))
                {
                    result = false;
                }
                else
                {
                    var p = tool.Check(last as ImplicitToolBase);

                    switch (p)
                    {
                        case Enums.AlongDirectionSemplificationCheckResult.GoOn:
                            // l'ultimo inserito è più avanti del tool => non lo inserisco
                            result = false;
                            break;
                        case Enums.AlongDirectionSemplificationCheckResult.BackOff:
                            list[lastIndex] = tool;
                            break;
                        default:
                            list.Add(tool);
                            break;
                    }
                }
            }
            else
            {
                list.Add(tool);
            }

            return result;
        }

        public static bool SmartAddRange(this List<BoundedImplicitFunction3d> list, List<BoundedImplicitFunction3d> tools, bool checkAll = false)
        {
            bool result = false;

            if (checkAll)
            {
                throw new NotImplementedException();
            }
            else
            {
                for (int i = 0; i < tools.Count; i++)
                {
                    if (i == 0)
                    {
                        result = list.SmartAdd(tools[i] as ImplicitToolBase);
                    }
                    else
                    {
                        list.Add(tools[i]);
                        result = true;
                    }
                }
            }

            return result;
        }

        public static bool RemoveByIndex(this List<BoundedImplicitFunction3d> list, int stepIndex)
        {
            bool result = false;

            if(list.Count > 0)
            {
                var n = list.RemoveAll((o) =>
                {
                    var res = false;

                    if ((o is IStepIndexed si) && (si.Index == stepIndex)) res = true;

                    return res;
                });

                if (n > 0) result = true;
            }

            return result;
        }
    }

}
