using g3;
using MaterialRemoval.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Geometry3D = HelixToolkit.Wpf.SharpDX.Geometry3D;
using MeshGeometry3D = HelixToolkit.Wpf.SharpDX.MeshGeometry3D;
using IntCollection = HelixToolkit.Wpf.SharpDX.IntCollection;
using Vector3 = SharpDX.Vector3;
using MaterialRemoval.Models;
using MaterialRemoval.Messages;
using MaterialRemoval.Helpers.UI;
using MaterialRemoval.Extensions;

namespace MaterialRemoval.ViewModels
{
    public class CornerPanelSectionViewMoldel : SidePanelSectionViewModel
    {
        private SafeGeneratedMeshData _safeNdSideMeshData = new SafeGeneratedMeshData();

        private Geometry3D _ndSideGeometry;

        private AxisAlignedBox3d _ndSideFilterBox;

        private ImplicitNaryDifference3d _ndProcessedSide;

        private int _processingNdSidePanel = 0;

        public CornerPanelSectionViewMoldel() : base()
        {
        }

        protected override void ResetProcessedSide()
        {
            GetSidesPositions(out PanelSectionPosition position, out PanelSectionPosition ndPosition);
            ResetProcessedSide(_processedSide, position);
            AdaptFaceDimensions(_processedSide.A as ImplicitFace, ref _sideFilterBox);
            ResetProcessedSide(_ndProcessedSide, ndPosition);
            AdaptFaceDimensions(_ndProcessedSide.A as ImplicitFace, ref _ndSideFilterBox);

        }

        protected override void ResetModel()
        {
            _ndSideGeometry = CreateNdSideModel();

            base.ResetModel();
        }

        protected override void InitializeModel()
        {
            _ndSideGeometry = CreateNdSideModel();

            base.InitializeModel();
        }

        private Geometry3D CreateSideModel()
        {
            Geometry3D geometry = null;

            switch (Position)
            {
                case PanelSectionPosition.CornerBottomLeft:
                case PanelSectionPosition.CornerBottomRight:
                    geometry = base.CreateSideModel(PanelSectionPosition.SideBottom);
                    break;
                case PanelSectionPosition.CornerTopLeft:
                case PanelSectionPosition.CornerTopRight:
                    geometry = base.CreateSideModel(PanelSectionPosition.SideTop);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return geometry;
        }

        private Geometry3D CreateNdSideModel()
        {
            Geometry3D geometry = null;

            switch (Position)
            {
                case PanelSectionPosition.CornerBottomLeft:
                case PanelSectionPosition.CornerTopLeft:
                    geometry = base.CreateSideModel(PanelSectionPosition.SideLeft);
                    break;
                case PanelSectionPosition.CornerBottomRight:
                case PanelSectionPosition.CornerTopRight:
                    geometry = base.CreateSideModel(PanelSectionPosition.SideRight);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return geometry;
        }

        protected override void UpdateGeometry()
        {
            var meshes = new MeshGeometry3D[] 
            { 
                _sectionGeometry as MeshGeometry3D, 
                _sideGeometry as MeshGeometry3D,
                _ndSideGeometry as MeshGeometry3D
            };
            
            Geometry = MeshGeometry3D.Merge(meshes);
        }

        protected override void InitializeProcessedSide()
        {
            GetSidesPositions(out PanelSectionPosition position, out PanelSectionPosition ndPosition);

            ImplicitFace face = CreateProcessedSide(position);
            ImplicitFace ndFace = CreateProcessedSide(ndPosition);

            AdaptFaceDimensions(face, ref _sideFilterBox);

            _processedSide = new ImplicitNaryDifference3d()
            {
                A = face,
                BSet = new List<BoundedImplicitFunction3d>()
            };

            AdaptFaceDimensions(ndFace, ref _ndSideFilterBox);

            _ndProcessedSide = new ImplicitNaryDifference3d()
            {
                A = ndFace,
                BSet = new List<BoundedImplicitFunction3d>()
            };
        }

        private void GetSidesPositions(out PanelSectionPosition position, out PanelSectionPosition ndPosition)
        {
            switch (Position)
            {

                case PanelSectionPosition.CornerBottomLeft:
                    position = PanelSectionPosition.SideBottom;
                    ndPosition = PanelSectionPosition.SideLeft;
                    break;
                case PanelSectionPosition.CornerBottomRight:
                    position = PanelSectionPosition.SideBottom;
                    ndPosition = PanelSectionPosition.SideRight;
                    break;
                case PanelSectionPosition.CornerTopLeft:
                    position = PanelSectionPosition.SideTop;
                    ndPosition = PanelSectionPosition.SideLeft;
                    break;
                case PanelSectionPosition.CornerTopRight:
                    position = PanelSectionPosition.SideTop;
                    ndPosition = PanelSectionPosition.SideRight;
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        protected override void ProcessPendingExtension(List<BoundedImplicitFunction3d> pendingTools)
        {
            base.ProcessPendingExtension(pendingTools);

            ProcessPendingExtensionImplementation(_ndProcessedSide, pendingTools, _ndSideFilterBox, (m) => GetMeshDataAsync(m, _safeNdSideMeshData));
        }

        protected override void OnBackStepMessageImplementation(int index)
        {
            ProcessRemoveByIndexExtensionImplementation(_ndProcessedSide,
                                                        _ndSideFilterBox,
                                                        (m, e) => GetMeshDataAsync(m, _safeNdSideMeshData, e),
                                                        index);

            base.OnBackStepMessageImplementation(index);
        }

        protected override void OnProcessPendingRemovalMessage(ProcessPendingRemovalMessage msg)
        {
            _safeNdSideMeshData.ExeAndReset((positions, triangleIndices, normals) =>
            {
                DispatcherHelperEx.CheckBeginInvokeOnUI(() =>
                {
                    _ndSideGeometry = new MeshGeometry3D()
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
            _ndProcessedSide.BSet.Add(rout);
        }

        protected override void ProcessPendingRout()
        {
            base.ProcessPendingRout();

            if (Interlocked.CompareExchange(ref _processingNdSidePanel, 1, 0) == 0)
            {
                Task.Run(() =>
                {
                    ProcessPendingRoutImplementation(_ndProcessedSide, _ndSideFilterBox, (m) => GetMeshDataAsync(m, _safeNdSideMeshData));
                    Interlocked.Exchange(ref _processingNdSidePanel, 0);
                });
            }
        }
    }
}
