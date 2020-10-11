using g3;
using MaterialRemoval.Enums;
using MaterialRemoval.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Geometry3D = HelixToolkit.Wpf.SharpDX.Geometry3D;
using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;
using IntCollection = HelixToolkit.Wpf.SharpDX.IntCollection;
using MaterialRemoval.Extensions;
using MaterialRemoval.Messages;
using MaterialRemoval.Helpers.UI;
using Vector3 = SharpDX.Vector3;

namespace MaterialRemoval.ViewModels
{
    public class SidePanelSectionViewModel : PanelSectionViewModel
    {
        private int _processingSidePanel = 0;

        private SafeGeneratedMeshData _safeSideMeshData = new SafeGeneratedMeshData();

        protected Geometry3D _sideGeometry;

        protected ImplicitNaryDifference3d _processedSide;

        protected AxisAlignedBox3d _sideFilterBox;

        public SidePanelSectionViewModel() : base()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            InitializeProcessedSide();
        }

        protected virtual void InitializeProcessedSide()
        {
            ImplicitFace face = CreateProcessedSide(Position);

            AdaptFaceDimensions(face);

            _processedSide = new ImplicitNaryDifference3d()
            {
                A = face,
                BSet = new List<BoundedImplicitFunction3d>()
            };
        }

        private void AdaptFaceDimensions(ImplicitFace face) => AdaptFaceDimensions(face, ref _sideFilterBox);

        protected static void AdaptFaceDimensions(ImplicitFace face, ref AxisAlignedBox3d sideFilterBox)
        {
            sideFilterBox = face.Bounds();
            sideFilterBox.Expand(0.0001);
            face.Width += 2.0;
            face.Height += 2.0;
        }

        protected ImplicitFace CreateProcessedSide(PanelSectionPosition position)
        {
            return new ImplicitFace()
            {
                Origin = Center.ToVector3d() + GetSideCenterOffset(position),
                N = GetSideNormal(position),
                U = GetSideUDirection(position),
                V = Vector3d.AxisZ,
                Width = GetSideWidth(position),
                Height = SizeZ
            };
        }

        protected override void InitializeModel()
        {
            _sideGeometry = CreateSideModel(Position);

            base.InitializeModel();
        }

        protected override void ResetProcessedPanel()
        {
            base.ResetProcessedPanel();

            ResetProcessedSide();
        }

        protected virtual void ResetProcessedSide()
        {
            ResetProcessedSide(_processedSide, Position);
            AdaptFaceDimensions(_processedSide.A as ImplicitFace);
        }

        protected void ResetProcessedSide(ImplicitNaryDifference3d processedSide, PanelSectionPosition position)
        {
            var face = processedSide.A as ImplicitFace;

            face.Origin = Center.ToVector3d() + GetSideCenterOffset(position);
            face.N = GetSideNormal(position);
            face.U = GetSideUDirection(position);
            face.V = Vector3d.AxisZ;
            face.Width = GetSideWidth(position);
            face.Height = SizeZ;

            processedSide.BSet.Clear();
        }

        protected override void ResetModel()
        {
            _sideGeometry = CreateSideModel(Position);

            base.ResetModel();
        }

        protected override void UpdateGeometry()
        {
            var meshes = new MeshGeometry3D[] { _sectionGeometry as MeshGeometry3D, _sideGeometry as MeshGeometry3D };
            Geometry = MeshGeometry3D.Merge(meshes);
        }

        protected Geometry3D CreateSideModel(PanelSectionPosition position) => CreateRect(position);


        private MeshGeometry3D CreateRect(PanelSectionPosition position)
        {
            var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();
            var dx = SizeX / 2.0;
            var dy = SizeY / 2.0;
            var dz = SizeZ / 2.0;

            switch (position)
            {
                case PanelSectionPosition.SideBottom:
                case PanelSectionPosition.CornerBottomLeft:
                case PanelSectionPosition.CornerBottomRight:
                    builder.AddQuad(new Vector3((float)(Center.X - dx), (float)(Center.Y - dy), (float)(Center.Z + dz)),
                                    new Vector3((float)(Center.X - dx), (float)(Center.Y - dy), (float)(Center.Z - dz)),
                                    new Vector3((float)(Center.X + dx), (float)(Center.Y - dy), (float)(Center.Z - dz)),
                                    new Vector3((float)(Center.X + dx), (float)(Center.Y - dy), (float)(Center.Z + dz)));
                    break;
                case PanelSectionPosition.SideTop:
                case PanelSectionPosition.CornerTopLeft:
                case PanelSectionPosition.CornerTopRight:
                    builder.AddQuad(new Vector3((float)(Center.X + dx), (float)(Center.Y + dy), (float)(Center.Z + dz)),
                                    new Vector3((float)(Center.X + dx), (float)(Center.Y + dy), (float)(Center.Z - dz)),
                                    new Vector3((float)(Center.X - dx), (float)(Center.Y + dy), (float)(Center.Z - dz)),
                                    new Vector3((float)(Center.X - dx), (float)(Center.Y + dy), (float)(Center.Z + dz)));
                    break;
                case PanelSectionPosition.SideRight:
                    builder.AddQuad(new Vector3((float)(Center.X + dx), (float)(Center.Y - dy), (float)(Center.Z + dz)),
                                    new Vector3((float)(Center.X + dx), (float)(Center.Y - dy), (float)(Center.Z - dz)),
                                    new Vector3((float)(Center.X + dx), (float)(Center.Y + dy), (float)(Center.Z - dz)),
                                    new Vector3((float)(Center.X + dx), (float)(Center.Y + dy), (float)(Center.Z + dz)));
                    break;
                case PanelSectionPosition.SideLeft:
                    builder.AddQuad(new Vector3((float)(Center.X - dx), (float)(Center.Y + dy), (float)(Center.Z + dz)),
                                    new Vector3((float)(Center.X - dx), (float)(Center.Y + dy), (float)(Center.Z - dz)),
                                    new Vector3((float)(Center.X - dx), (float)(Center.Y - dy), (float)(Center.Z - dz)),
                                    new Vector3((float)(Center.X - dx), (float)(Center.Y - dy), (float)(Center.Z + dz)));
                    break;

                default:
                    throw new ArgumentException("The argument must be signle side position!");
            }

            return builder.ToMesh();
        }

        private void CutSectionSide(DMesh3 mesh, Vector3d origin, Vector3d direction, bool fill)
        {
            MeshPlaneCut cut = new MeshPlaneCut(mesh, origin, direction);
            bool bc = cut.Cut();

            if(fill)
            {
                PlanarHoleFiller filler = new PlanarHoleFiller(cut) { FillTargetEdgeLen = _cubeSize };
                bool bf = filler.Fill();
            }
        }

        private Vector3d GetSideNormal(PanelSectionPosition position)
        {
            Vector3d n;

            switch (position)
            {
                case PanelSectionPosition.SideBottom:
                    n = -Vector3d.AxisY;
                    break;
                case PanelSectionPosition.SideTop:
                    n = Vector3d.AxisY;
                    break;
                case PanelSectionPosition.SideRight:
                    n = Vector3d.AxisX;
                    break;
                case PanelSectionPosition.SideLeft:
                    n = -Vector3d.AxisX;
                    break;
                default:
                    throw new ArgumentException();
            }

            return n;
        }

        private Vector3d GetSideCenterOffset(PanelSectionPosition position)
        {
            Vector3d d;

            switch (position)
            {
                case PanelSectionPosition.SideBottom:
                    d = new Vector3d(0.0, -SizeY / 2.0, 0.0);
                    break;
                case PanelSectionPosition.SideTop:
                    d = new Vector3d(0.0, SizeY / 2.0, 0.0);
                    break;
                case PanelSectionPosition.SideRight:
                    d = new Vector3d(SizeX / 2.0, 0.0, 0.0);
                    break;
                case PanelSectionPosition.SideLeft:
                    d = new Vector3d(-SizeX / 2.0, 0.0, 0.0);
                    break;
                default:
                    throw new ArgumentException();
            }

            return d;
        }

        private Vector3d GetSideUDirection(PanelSectionPosition position)
        {
            Vector3d uDir;

            switch (position)
            {
                case PanelSectionPosition.SideBottom:
                    uDir = Vector3d.AxisX;
                    break;
                case PanelSectionPosition.SideTop:
                    uDir = -Vector3d.AxisX;
                    break;
                case PanelSectionPosition.SideRight:
                    uDir = Vector3d.AxisY;
                    break;
                case PanelSectionPosition.SideLeft:
                    uDir = -Vector3d.AxisY;
                    break;
                default:
                    throw new ArgumentException();
            }

            return uDir;
        }

        private double GetSideWidth(PanelSectionPosition position)
        {
            double w = 0.0;

            switch (position)
            {
                case PanelSectionPosition.SideBottom:
                case PanelSectionPosition.SideTop:
                    w = SizeX;
                    break;
                case PanelSectionPosition.SideRight:
                case PanelSectionPosition.SideLeft:
                    w = SizeY;
                    break;
                default:
                    throw new ArgumentException();
            }

            return w;
        }

        protected override void ProcessPendingExtension(List<BoundedImplicitFunction3d> pendingTools)
        {
            ProcessPendingExtensionImplementation(_processedSide, pendingTools, _sideFilterBox, (m) => GetMeshDataAsync(m, _safeSideMeshData));
        }

        protected void ProcessPendingExtensionImplementation(ImplicitNaryDifference3d processedSide, List<BoundedImplicitFunction3d> pendingTools, AxisAlignedBox3d filterBox, Action<DMesh3> setProcessedMesh)
        {
            var bounds = processedSide.Bounds();
            var indexList = new List<int>();

            for (int i = 0; i < pendingTools.Count; i++)
            {
                if (bounds.Intersects(pendingTools[i].Bounds())) indexList.Add(i);
            }

            if (indexList.Count > 0)
            {
                foreach (var idx in indexList) processedSide.BSet.Add(pendingTools[idx]);

                ProcessSideMesh(processedSide, filterBox, setProcessedMesh);
            }
        }

        private void ProcessSideMesh(ImplicitNaryDifference3d processedSide, AxisAlignedBox3d filterBox, Action<DMesh3> setProcessedMesh)
        {
            DMesh3 mesh = GenerateMeshBase(processedSide, filterBox);

            MeshPlaneCut cut = new MeshPlaneCut(mesh, Center.ToVector3d() + (Vector3d.AxisZ * SizeZ / 2.0), Vector3d.AxisZ);
            bool bc = cut.Cut();

            if (!mesh.IsCompact) mesh.CompactInPlace();

            setProcessedMesh(mesh);
        }

        protected override void OnBackStepMessageImplementation(int index)
        {
            ProcessRemoveByIndexExtensionImplementation(_processedSide,
                                                        _sideFilterBox,
                                                        (m, e) => GetMeshDataAsync(m, _safeSideMeshData, e),
                                                        index);

            base.OnBackStepMessageImplementation(index);
        }

        protected void ProcessRemoveByIndexExtensionImplementation(ImplicitNaryDifference3d processedSide, AxisAlignedBox3d filterBox, Action<DMesh3, ManualResetEventSlim> setProcessedMesh, int index)
        {
            var process = processedSide.BSet.RemoveByIndex(index);

            if (process)
            {
                var mres = new ManualResetEventSlim();

                ProcessSideMesh(processedSide, filterBox, (m) => setProcessedMesh(m, mres));

                mres.Wait(2000);
                mres.Reset();
            }
        }

        private DMesh3 GenerateMeshBase(BoundedImplicitFunction3d root, AxisAlignedBox3d filterBox)
        {
            MarchingCubes c = new MarchingCubes();
            c.Implicit = root;
            c.RootMode = MarchingCubes.RootfindingModes.LerpSteps;      // cube-edge convergence method
            c.RootModeSteps = 5;                                        // number of iterations
            c.Bounds = filterBox;//_sideFilterBox;
            c.CubeSize = _cubeSize / 4.0;
            c.Generate();
            MeshNormals.QuickCompute(c.Mesh);                           // generate normals

            return c.Mesh;
        }

        protected override void OnProcessPendingRemovalMessage(ProcessPendingRemovalMessage msg)
        {
            _safeSideMeshData.ExeAndReset((positions, triangleIndices, normals) =>
            {
                DispatcherHelperEx.CheckBeginInvokeOnUI(() =>
                {
                    _sideGeometry = new MeshGeometry3D()
                    {
                        Positions = positions,
                        TriangleIndices = new IntCollection(triangleIndices),
                        Normals = normals
                    };
                });
            });

            base.OnProcessPendingRemovalMessage(msg);
        }

        protected override void AddRautingToProcessedPanel(SectionRoutToolMoveMessage msg, ImplicitRouting rout)
        {
            base.AddRautingToProcessedPanel(msg, rout);
            _processedSide.BSet.Add(rout);
        }

        protected override void ProcessPendingRout()
        {
            base.ProcessPendingRout();

            if (Interlocked.CompareExchange(ref _processingSidePanel, 1, 0) == 0)
            {
                Task.Run(() =>
                {
                    ProcessPendingRoutImplementation(_processedSide, _sideFilterBox, (m) => GetMeshDataAsync(m, _safeSideMeshData));
                    Interlocked.Exchange(ref _processingSidePanel, 0);
                });
            }            
        }

        protected void ProcessPendingRoutImplementation(ImplicitNaryDifference3d processedSide, AxisAlignedBox3d filterBox, Action<DMesh3> setProcessedMesh)
        {
            DMesh3 mesh = GenerateMeshBase(processedSide, filterBox);

            MeshPlaneCut cut = new MeshPlaneCut(mesh, Center.ToVector3d() + (Vector3d.AxisZ * SizeZ / 2.0), Vector3d.AxisZ);
            bool bc = cut.Cut();

            if (!mesh.IsCompact) mesh.CompactInPlace();

            setProcessedMesh(mesh);
        }

    }
}
