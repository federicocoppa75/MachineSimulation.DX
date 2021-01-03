﻿using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using MaterialRemoval.Models;
using MaterialRemoval.Messages;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Rect3D = System.Windows.Media.Media3D.Rect3D;
using MaterialRemoval.Enums;
using g3;
using MaterialRemoval.Extensions;
using MachineElements.ViewModels.Interfaces.Panel;
using MachineElements.ViewModels.Interfaces.Messages.Panel;
using IStepObserver = MachineElements.ViewModels.Interfaces.Steps.IStepObserver;
using GalaSoft.MvvmLight.Ioc;
using StepExecutionDirection =  MachineElements.ViewModels.Interfaces.Enums.StepExecutionDirection;
using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX;
using System.Linq;

namespace MaterialRemoval.ViewModels
{
    public class PanelViewModel : Base.PanelViewModel, IPanelViewModel
    {
        const int _numCells = 16;

        int _nxSection;
        int _nySection;

        private double _cornerX;
        private double _cornerY;
        private double _cornerZ;

        private ConcurrentDictionary<int, ImplicitRouting> _lastRoutings = new ConcurrentDictionary<int, ImplicitRouting>();

        private IStepObserver _stepObserver;

        public double SizeX { get; set; }
        public double SizeY { get; set; }
        public double SizeZ { get; set; }
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double CenterZ { get; set; }
        public Rect3D Bound => GetBound();

        public PanelViewModel()
        {
            MessengerInstance.Register<ToolMoveMessage>(this, OnToolMoveMessage);
            MessengerInstance.Register<RoutToolMoveMessage>(this, OnRoutToolMoveMessage);
            MessengerInstance.Register<InjectMessage>(this, OnInjectMessage);
            MessengerInstance.Register<InsertMessage>(this, OnInsertMessage);
            MessengerInstance.Register<PanelExportRequestMessage>(this, OnPanelExportRequestMessage);
            MessengerInstance.Register<PanelPresenceRequestMessage>(this, m => m?.Confirm());

            _stepObserver = SimpleIoc.Default.GetInstance<IStepObserver>();
        }

        public void Initialize()
        {
            if (Children.Count > 0) Children.Clear();
            if (_lastRoutings.Count > 0) _lastRoutings.Clear();

            InitializeSectionsNumber();

            double xSectionSize = SizeX / _nxSection;
            double ySectionSize = SizeY / _nySection;
            double xStartOffset = - SizeX / 2.0;
            double yStartOffset = - SizeY / 2.0;

            _cornerX = CenterX + xStartOffset;
            _cornerY = CenterY + yStartOffset;
            _cornerZ = CenterZ - SizeZ / 2.0;

            for (int i = 0; i < _nxSection; i++)
            {
                var xCenter = _cornerX + xSectionSize / 2.0 + xSectionSize * i;

                for (int j = 0; j < _nySection; j++)
                {
                    var yCenter = _cornerY + ySectionSize / 2.0 + ySectionSize * j;
                    var center = new Point3D(xCenter, yCenter, CenterZ);
                    var section = CreatePanelSection(center, xSectionSize, ySectionSize, i, j);

                    Children.Add(section);
                }
            }
        }

        private void InitializeSectionsNumber()
        {
            const int sectionsX100mm = 3;
            _nxSection = (int)Math.Ceiling(SizeX / 100.0) * sectionsX100mm;
            _nySection = (int)Math.Ceiling(SizeY / 100.0) * sectionsX100mm;
        }

        private PanelSectionViewModel CreatePanelSection(Point3D center, double xSectionSize, double ySectionSize, int i, int j)
        {
            var positon = GetSctionPosition(i, j);
            PanelSectionViewModel panelSection = null;

            PanelSectionViewModelFactory.NumCells = _numCells;
            PanelSectionViewModelFactory.SizeZ = SizeZ;

            switch (positon)
            {
                case PanelSectionPosition.Center:
                    //panelSection = CreateCenterPanelSection(center, xSectionSize, ySectionSize, i, j);
                    panelSection = PanelSectionViewModelFactory.CreateCenterPanelSection(center, xSectionSize, ySectionSize, i, j);
                    break;
                case PanelSectionPosition.SideBottom:
                case PanelSectionPosition.SideTop:
                case PanelSectionPosition.SideRight:
                case PanelSectionPosition.SideLeft:
                    //panelSection = CreateSidePanelSection(center, xSectionSize, ySectionSize, i, j, positon);
                    panelSection = PanelSectionViewModelFactory.CreateSidePanelSection(center, xSectionSize, ySectionSize, i, j, positon);
                    break;
                case PanelSectionPosition.CornerBottomLeft:
                case PanelSectionPosition.CornerBottomRight:
                case PanelSectionPosition.CornerTopLeft:
                case PanelSectionPosition.CornerTopRight:
                    //panelSection = CreateCornerPanelSection(center, xSectionSize, ySectionSize, i, j, positon);
                    panelSection = PanelSectionViewModelFactory.CreateCornerPanelSection(center, xSectionSize, ySectionSize, i, j, positon);
                    break;
                default:
                    break;
            }

            return panelSection;
        }

        //private PanelSectionViewModel CreateSidePanelSection(Point3D center, double xSectionSize, double ySectionSize, int i, int j, PanelSectionPosition position)
        //{
        //    var section = new SidePanelSectionViewModel()
        //    {
        //        XSectionIndex = i,
        //        YSectionIndex = j,
        //        Position = position,
        //        NumCells = _numCells,
        //        SizeX = xSectionSize,
        //        SizeY = ySectionSize,
        //        SizeZ = SizeZ,
        //        Center = center,
        //        Visible = true
        //    };

        //    section.Initialize();
        //    return section;
        //}

        //private PanelSectionViewModel CreateCornerPanelSection(Point3D center, double xSectionSize, double ySectionSize, int i, int j, PanelSectionPosition position)
        //{
        //    var section = new CornerPanelSectionViewMoldel()
        //    {
        //        XSectionIndex = i,
        //        YSectionIndex = j,
        //        Position = position,
        //        NumCells = _numCells,
        //        SizeX = xSectionSize,
        //        SizeY = ySectionSize,
        //        SizeZ = SizeZ,
        //        Center = center,
        //        Visible = true
        //    };

        //    section.Initialize();
        //    return section;
        //}


        //private PanelSectionViewModel CreateCenterPanelSection(Point3D center, double xSectionSize, double ySectionSize, int i, int j)
        //{
        //    var section = new PanelSectionViewModel()
        //    {
        //        XSectionIndex = i,
        //        YSectionIndex = j,
        //        Position = PanelSectionPosition.Center,
        //        NumCells = _numCells,
        //        SizeX = xSectionSize,
        //        SizeY = ySectionSize,
        //        SizeZ = SizeZ,
        //        Center = center,
        //        Visible = true
        //    };

        //    section.Initialize();
        //    return section;
        //}

        private PanelSectionPosition GetSctionPosition(int i, int j)
        {
            PanelSectionPosition result = PanelSectionPosition.Center;
            bool isLeft = i == 0;
            bool isRight = i == _nxSection - 1;
            bool isBottom = j == 0;
            bool isTop = j == _nySection - 1;

            if(isLeft)
            {
                if (isBottom) result = PanelSectionPosition.CornerBottomLeft;
                else if (isTop) result = PanelSectionPosition.CornerTopLeft;
                else result = PanelSectionPosition.SideLeft;
            }
            else if(isRight)
            {
                if (isBottom) result = PanelSectionPosition.CornerBottomRight;
                else if (isTop) result = PanelSectionPosition.CornerTopRight;
                else result = PanelSectionPosition.SideRight;
            }
            else
            {
                if (isBottom) result = PanelSectionPosition.SideBottom;
                else if (isTop) result = PanelSectionPosition.SideTop;
                else result = PanelSectionPosition.Center;
            }

            return result;
        }

        private void OnToolMoveMessage(ToolMoveMessage msg)
        {
            if (_stepObserver.Direction == StepExecutionDirection.Back) return;

            //var position = PanelModel.Transform.Inverse.Transform(msg.Position);
            //var tool = ImplicitToolFactory.Create(position.ToVector3d(), msg.Direction.ToVector3d(), msg.Length, msg.Radius);
            var tool = ImplicitToolFactory.Create(msg.Position, msg.Direction, msg.Length, msg.Radius);
            var panel = new AxisAlignedBox3d(new Vector3d(_cornerX, _cornerY, _cornerZ), new Vector3d(_cornerX + SizeX, _cornerY + SizeY, _cornerZ + SizeZ));

            tool.Index = _stepObserver.Index;
            //{
            //    var builder = new MeshBuilder();

            //    builder.AddSphere(msg.Position, 10);
            //    PanelModel.Children.Add(new GeometryModel3D() { Geometry = builder.ToMesh(), Material = MaterialHelper.CreateMaterial(Colors.Yellow) });
            //}

            Task.Run(() =>
            {
                var toolBound = tool.Bounds();

                if(panel.Intersects(toolBound))
                {
                    var intersect = panel.Intersect(toolBound);
                    var xMinIndex = GetSectionIndex(intersect.Min.x, panel.Min.x, panel.Max.x, _nxSection);
                    var xMaxIndex = GetSectionIndex(intersect.Max.x, panel.Min.x, panel.Max.x, _nxSection);
                    var yMinIndex = GetSectionIndex(intersect.Min.y, panel.Min.y, panel.Max.y, _nySection);
                    var yMaxIndex = GetSectionIndex(intersect.Max.y, panel.Min.y, panel.Max.y, _nySection);

                    if (xMinIndex < 0) xMinIndex = 0;
                    if (yMinIndex < 0) yMinIndex = 0;

                    for (int i = xMinIndex; i <= xMaxIndex; i++)
                    {
                        for (int j = yMinIndex; j <= yMaxIndex; j++)
                        {
                            MessengerInstance.Send(new SectionToolMoveMessage() { XSectionIndex = i, YSectionIndex = j, Tool = tool });
                        }
                    }
                }
            });
        }

        private void OnRoutToolMoveMessage(RoutToolMoveMessage msg)
        {
            if (_stepObserver.Direction == StepExecutionDirection.Back) return;

            var tool = ImplicitToolFactory.Create(msg.Position, msg.Direction, msg.Length, msg.Radius);
            var panel = new AxisAlignedBox3d(new Vector3d(_cornerX, _cornerY, _cornerZ), new Vector3d(_cornerX + SizeX, _cornerY + SizeY, _cornerZ + SizeZ));

            //DispatcherHelper.CheckBeginInvokeOnUI(() =>
            //{
            //    var builder = new MeshBuilder();

            //    builder.AddSphere(msg.Position, 10);
            //    PanelModel.Children.Add(new GeometryModel3D() { Geometry = builder.ToMesh(), Material = MaterialHelper.CreateMaterial(Colors.Yellow) });
            //});

            Task.Run(() =>
            {
                var toolBound = tool.Bounds();

                if (panel.Intersects(toolBound))
                {
                    var routing = _lastRoutings.GetOrAdd(msg.ToolId, (id) => ImplicitRoutingFactory.Create(id, msg.Direction.ToVector3d(), msg.Length, msg.Radius));
                    var pt = msg.Position.ToVector3d();
                    var box = routing.Add(ref pt);

                    var intersect = panel.Intersect(box);
                    var xMinIndex = GetSectionIndex(intersect.Min.x, panel.Min.x, panel.Max.x, _nxSection);
                    var xMaxIndex = GetSectionIndex(intersect.Max.x, panel.Min.x, panel.Max.x, _nxSection);
                    var yMinIndex = GetSectionIndex(intersect.Min.y, panel.Min.y, panel.Max.y, _nySection);
                    var yMaxIndex = GetSectionIndex(intersect.Max.y, panel.Min.y, panel.Max.y, _nySection);

                    if (xMinIndex < 0) xMinIndex = 0;
                    if (yMinIndex < 0) yMinIndex = 0;

                    for (int i = xMinIndex; i <= xMaxIndex; i++)
                    {
                        for (int j = yMinIndex; j <= yMaxIndex; j++)
                        {
                            MessengerInstance.Send(new SectionRoutToolMoveMessage() { XSectionIndex = i, YSectionIndex = j, Rout = routing });
                        }
                    }
                }
                else
                {
                    _lastRoutings.TryRemove(msg.ToolId, out ImplicitRouting ir);
                }
            });
        }

        private static int GetSectionIndex(double value, double min, double max, int nSections)
        {
            var f = nSections * (value - min) / (max - min);
            var i= (int)Math.Ceiling(f) - 1;

            return i;
        }

        private Rect3D GetBound() => new Rect3D(_cornerX, _cornerY, _cornerZ, SizeX, SizeY, SizeZ);

        private void OnInjectMessage(InjectMessage msg)
        {
            var ie = msg.InjectElement;

            ie.Parent = this;
            Children.Add(ie);
        }

        private void OnInsertMessage(InsertMessage msg)
        {
            var ie = msg.InsertElement;

            ie.Parent = this;
            Children.Add(ie);
        }

        private void OnPanelExportRequestMessage(PanelExportRequestMessage msg)
        {
            var list = new List<Geometry3D>();

            MessengerInstance.Send(new PanelExportMessage() { AddSectionGeometry = (g) => list.Add(g) });

            var dList = list.Select(g => (g as MeshGeometry3D).ToDMesh3()).ToList();

            StandardMeshWriter.WriteMeshes(msg.FileName, dList, WriteOptions.Defaults);
        }
    }
}
