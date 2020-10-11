using g3;
using GalaSoft.MvvmLight;
using MaterialRemoval.Enums;
using MaterialRemoval.Extensions;
using MaterialRemoval.Messages;
using MaterialRemoval.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Geometry3D = HelixToolkit.Wpf.SharpDX.Geometry3D;
using HelixToolkit.Wpf.SharpDX;
using MaterialRemoval.Helpers.UI;
using Vector3 = SharpDX.Vector3;
using IResettable = MachineElements.ViewModels.Interfaces.IResettable;
using MachineElements.ViewModels.Interfaces.Messages.Steps;

namespace MaterialRemoval.ViewModels
{
    public class PanelSectionViewModel : Base.PanelSectionViewModel, IResettable
    {
        protected class SafeGeneratedMeshData
        {
            ConcurrentQueue<Tuple<Vector3Collection, IntCollection, Vector3Collection>> _queue = new ConcurrentQueue<Tuple<Vector3Collection, IntCollection, Vector3Collection>>();

            public void Set(Vector3Collection positions, IntCollection triangleIndeces, Vector3Collection normals)
            {
                _queue.Enqueue(new Tuple<Vector3Collection, IntCollection, Vector3Collection>(positions, triangleIndeces, normals));
            }

            public void ExeAndReset(Action<Vector3Collection, IntCollection, Vector3Collection> action)
            {
                if (!_queue.IsEmpty && _queue.TryDequeue(out Tuple<Vector3Collection, IntCollection, Vector3Collection> t))
                {
                    action(t.Item1, t.Item2, t.Item3);
                }
            }
        }

        private static int _seedId = 0;

        protected double _processingOffset = 2.0;

        private object _queueLock = new object();

        private int _processingPanel = 0;

        protected List<BoundedImplicitFunction3d> _pendingTools;

        private ImplicitNaryDifference3d _processedPanel;

        protected AxisAlignedBox3d _filterBox;

        protected double _cubeSize;

        protected Vector3d _min;

        protected Vector3d _max;

        protected Geometry3D _sectionGeometry;

        private SafeGeneratedMeshData _generatedMeshData = new SafeGeneratedMeshData();

        private ImplicitRouting _lastRouting;

        private static object _routLock = new object();

        public int XSectionIndex { get; set; }

        public int YSectionIndex { get; set; }

        public PanelSectionPosition Position { get; set; }

        public Point3D Center { get; set; }

        public double SizeX { get; set; }

        public double SizeY { get; set; }

        public double SizeZ { get; set; }

        private int _numcells;
        public int NumCells
        {
            get => _numcells;
            set => _numcells = value;
        }

        public PanelSectionViewModel() : base()
        {
            //Id = _seedId++;
            MessengerInstance.Register<SectionToolMoveMessage>(this, OnSectionToolMoveMessage);
            MessengerInstance.Register<ProcessPendingRemovalMessage>(this, OnProcessPendingRemovalMessage);
            MessengerInstance.Register<SectionRoutToolMoveMessage>(this, OnSectionRoutToolMoveMessage);
            MessengerInstance.Register<BackStepMessage>(this, OnBackStepMessage);
        }

        public virtual void Initialize()
        {
            InitializeMinMax();

            InitializeMeshGenerationData();

            InitializeProcessedPanel();

            InitializeModel();
        }

        public virtual void Reset()
        {
            InitializeMinMax();
            InitializeMeshGenerationData();
            ResetProcessedPanel();
            ResetModel();
        }

        protected virtual void ResetProcessedPanel()
        {
            var offset = _processingOffset;
            var offsetV = new Vector3d(offset, offset, 0.0);
            var ind = _processedPanel.A as ImplicitBox3d;

            ind.Box = new Box3d(new AxisAlignedBox3d(_min - offsetV, _max + offsetV));
            _processedPanel.BSet.Clear();
        }

        protected virtual void ResetModel()
        {
            _sectionGeometry = CreateSectonModel();
            UpdateGeometry();
        }

        protected void InitializeMinMax()
        {
            var p = Center.ToVector3d() - new Vector3d(SizeX / 2.0, SizeY / 2.0, SizeZ / 2.0);
            var d = new Vector3d(SizeX, SizeY, SizeZ);

            _min = p;
            _max = p + d;
        }

        protected virtual void InitializeMeshGenerationData()
        {
            InitializeCubeSize();

            _filterBox = new AxisAlignedBox3d(_min, _max);
            _filterBox.Max.x -= _cubeSize / 2.0;
            _filterBox.Max.y -= _cubeSize / 2.0;
        }

        protected void InitializeCubeSize()
        {
            var filterBox = new AxisAlignedBox3d(_min, _max);
            _cubeSize = filterBox.MaxDim / _numcells;
        }

        protected void InitializeProcessedPanel()
        {
            var offset = _processingOffset;
            var offsetV = new Vector3d(offset, offset, 0.0);

            _processedPanel = new ImplicitNaryDifference3d()
            {
                A = new ImplicitBox3d()
                {
                    Box = new Box3d(new AxisAlignedBox3d(_min - offsetV, _max + offsetV))
                },
                BSet = new List<BoundedImplicitFunction3d>()
            };
        }

        protected virtual void InitializeModel()
        {
            _sectionGeometry = CreateSectonModel();
            UpdateGeometry();

            var material = PhongMaterials.BlackPlastic;

            material.DiffuseColor = PhongMaterials.Orange.DiffuseColor;
            Material = material;
        }

        protected Geometry3D CreateSectonModel()
        {
            var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();

            builder.AddCubeFace(Center.ToVector3(), Vector3.UnitZ, Vector3.UnitX, SizeZ, SizeY, SizeX);
            builder.AddCubeFace(Center.ToVector3(), -Vector3.UnitZ, Vector3.UnitX, SizeZ, SizeY, SizeX);

            return builder.ToMesh();
        }

        private void OnSectionToolMoveMessage(SectionToolMoveMessage msg)
        {
            if ((msg.XSectionIndex == XSectionIndex) && (msg.YSectionIndex == YSectionIndex))
            {
                var tool = msg.Tool;

                Task.Run(() =>
                {
                    bool process = false;

                    lock (_queueLock)
                    {
                        if (_pendingTools == null) _pendingTools = new List<BoundedImplicitFunction3d>();

                        process = _pendingTools.SmartAdd(tool);
                    }

                    if(process) Task.Run(() => ProcessPendings());
                });
            }
        }

        private void OnSectionRoutToolMoveMessage(SectionRoutToolMoveMessage msg)
        {
            if ((msg.XSectionIndex == XSectionIndex) && (msg.YSectionIndex == YSectionIndex))
            {
                var rout = msg.Rout;

                Task.Run(() =>
                {
                    bool add = true;

                    lock (_routLock)
                    {
                        if ((_lastRouting != null) && (_lastRouting.Id == rout.Id) && (_lastRouting.ToolId == rout.ToolId))
                        {
                            var lastIndex = _processedPanel.BSet.Count - 1;
                            var last = (lastIndex >= 0) ? _processedPanel.BSet[lastIndex] : null;

                            if ((last != null) && (last is ImplicitRouting ir) && (ir.Id == rout.Id) && (ir.ToolId == rout.ToolId))
                            {
                                //_processedPanel.BSet[lastIndex] = msg.Rout;
                                // teoricamente l'oggetto dovrebbe essere già aggiornato con il nuovo tool
                                add = false;
                            }
                        }

                        if (add)
                        {
                            AddRautingToProcessedPanel(msg, rout);
                        }
                    }

                    Task.Run(() => ProcessPendingRout());
                });
            }
        }

        private void OnBackStepMessage(BackStepMessage msg)
        {
            var index = msg.Index;

            if (Interlocked.CompareExchange(ref _processingPanel, 1, 0) == 0)
            {
                Task.Run(() =>
                {
                    OnBackStepMessageImplementation(index);

                    Interlocked.Exchange(ref _processingPanel, 0);
                });
            }
        }

        protected virtual void OnBackStepMessageImplementation(int index)
        {
            var process = _processedPanel.BSet.RemoveByIndex(index);

            if (process)
            {
                GenerateMesh(_processedPanel);
                ProcessPendingRemoval();
            }
        }

        protected virtual void AddRautingToProcessedPanel(SectionRoutToolMoveMessage msg, ImplicitRouting rout)
        {
            _processedPanel.BSet.Add(msg.Rout);
            _lastRouting = rout;
        }

        protected virtual void OnProcessPendingRemovalMessage(ProcessPendingRemovalMessage msg) => ProcessPendingRemoval();

        protected virtual void ProcessPendingRemoval()
        {
            _generatedMeshData.ExeAndReset((positions, triangleIndices, normals) =>
           {
               DispatcherHelperEx.CheckBeginInvokeOnUI(() =>
               {
                   _sectionGeometry = new MeshGeometry3D()
                   {
                       Positions = positions,
                       TriangleIndices = triangleIndices,
                       Normals = normals
                   };

                   UpdateGeometry();
               });
           });
        }

        protected virtual void UpdateGeometry() => Geometry = _sectionGeometry;

        private void ProcessPendings()
        {
            if (Interlocked.CompareExchange(ref _processingPanel, 1, 0) == 0)
            {
                Task.Run(() =>
                {
                    List<BoundedImplicitFunction3d> pendingTools = null;

                    lock (_queueLock)
                    {
                        pendingTools = _pendingTools;
                        _pendingTools = null;
                    }

                    if (pendingTools != null)
                    {
                        var added = _processedPanel.BSet.SmartAddRange(pendingTools);

                        if (added)
                        {
                            GenerateMesh(_processedPanel);
                            ProcessPendingExtension(pendingTools);
                        }
                    }

                    Interlocked.Exchange(ref _processingPanel, 0);
                });
            }
        }

        protected virtual void ProcessPendingRout()
        {
            if (Interlocked.CompareExchange(ref _processingPanel, 1, 0) == 0)
            {
                Task.Run(() =>
                {
                    GenerateMesh(_processedPanel);
                    Interlocked.Exchange(ref _processingPanel, 0);
                });
            }
        }

        private void GenerateMesh(BoundedImplicitFunction3d root)
        {
            DMesh3 mesh = GenerateMeshBase(root);

            if (!mesh.IsCompact) mesh.CompactInPlace();

            GetMeshDataAsync(mesh, _generatedMeshData);
        }

        private DMesh3 GenerateMeshBase(BoundedImplicitFunction3d root)
        {
            MarchingCubes c = new MarchingCubes();
            c.Implicit = root;
            c.RootMode = MarchingCubes.RootfindingModes.LerpSteps;      // cube-edge convergence method
            c.RootModeSteps = 5;                                        // number of iterations
            c.Bounds = _filterBox;
            c.CubeSize = _cubeSize;
            c.Generate();
            MeshNormals.QuickCompute(c.Mesh);                           // generate normals

            return c.Mesh;
        }

        protected virtual void ProcessPendingExtension(List<BoundedImplicitFunction3d> pendingTools){ }

        protected Task<Vector3Collection> GetPositionsAsync(DMesh3 mesh)
        {
            return Task.Run(() =>
            {
                var positions = new Vector3Collection(mesh.VerticesRefCounts.count);
                var vertices = mesh.VerticesBuffer;

                foreach (int vId in mesh.VerticesRefCounts)
                {
                    int i = vId * 3;
                    positions.Add(new SharpDX.Vector3((float)vertices[i], (float)vertices[i + 1], (float)vertices[i + 2]));
                }

                return positions;
            });
        }

        protected Task<IntCollection> GetTriangleIndicesAsync(DMesh3 mesh)
        {
            return Task.Run(() =>
            {
                var tringleindices = new IntCollection(mesh.TrianglesRefCounts.count);
                var triangles = mesh.TrianglesBuffer;

                foreach (int tId in mesh.TrianglesRefCounts)
                {
                    int i = tId * 3;
                    tringleindices.Add(triangles[i]);
                    tringleindices.Add(triangles[i + 1]);
                    tringleindices.Add(triangles[i + 2]);
                }

                return tringleindices;
            });
        }

        protected Task<Vector3Collection> GetNormalsAsync(DMesh3 mesh)
        {
            return Task.Run(() =>
            {
                var normalsList = new Vector3Collection(mesh.VerticesRefCounts.count);
                var normals = mesh.NormalsBuffer;

                foreach (int vId in mesh.VerticesRefCounts)
                {
                    int i = vId * 3;
                    normalsList.Add(new SharpDX.Vector3(normals[i], normals[i + 1], normals[i + 2]));
                }

                return normalsList;
            });
        }

        protected Task GetMeshDataAsync(DMesh3 mesh, SafeGeneratedMeshData meshData, ManualResetEventSlim eventToSet = null)
        {
            var ets = eventToSet;
            var md = meshData;
            Vector3Collection positions = null;
            IntCollection triangleIndices = null;
            Vector3Collection normals = null;
            var tasks = new Task[]
            {
                GetPositionsAsync(mesh).ContinueWith((t) => positions = t.Result),
                GetTriangleIndicesAsync(mesh).ContinueWith((t) => triangleIndices = t.Result),
                GetNormalsAsync(mesh).ContinueWith((t) => normals = t.Result)
            };

            return Task.WhenAll(tasks)
                       .ContinueWith((t) =>
                        {
                            md.Set(positions, triangleIndices, normals);
                            ets?.Set();
                        });
        }
    }
}
