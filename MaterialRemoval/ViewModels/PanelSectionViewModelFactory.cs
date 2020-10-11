using MaterialRemoval.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Point3D = System.Windows.Media.Media3D.Point3D;

namespace MaterialRemoval.ViewModels
{
    internal static class PanelSectionViewModelFactory
    {
        struct SectionIndex
        {
            static int factor = 1000;

            int index;

            public void SetIndex(int xIndex, int yIndex)
            {
                index = xIndex * factor + yIndex;
            }

            public int XIndex => Math.DivRem(index, factor, out int res);

            public int YIndex
            {
                get
                {
                    var q = Math.DivRem(index, factor, out int res);

                    return res;
                }
            }

            public override int GetHashCode() => index.GetHashCode();
            public override string ToString() => $"({XIndex},{YIndex})";
            public override bool Equals(object obj)
            {
                if(obj is SectionIndex s)
                {
                    return Equals(s);
                }
                else
                {
                    return false;
                }
            }

            public bool Equals(SectionIndex s) => index == s.index;

        }

        static Dictionary<SectionIndex, PanelSectionViewModel> _sectionsDictionary = new Dictionary<SectionIndex, PanelSectionViewModel>();
        static Dictionary<int, SidePanelSectionViewModel> _bottomSideSectionsDictionary = new Dictionary<int, SidePanelSectionViewModel>();
        static Dictionary<int, SidePanelSectionViewModel> _topSideSectionsDictionary = new Dictionary<int, SidePanelSectionViewModel>();
        static Dictionary<int, SidePanelSectionViewModel> _rightSideSectionsDictionary = new Dictionary<int, SidePanelSectionViewModel>();
        static Dictionary<int, SidePanelSectionViewModel> _leftSideSectionsDictionary = new Dictionary<int, SidePanelSectionViewModel>();
        static Dictionary<PanelSectionPosition, CornerPanelSectionViewMoldel> _cornerSectionsDictionary = new Dictionary<PanelSectionPosition, CornerPanelSectionViewMoldel>();

        static public int NumCells { get; set; }

        static public double SizeZ { get; set; }

        static PanelSectionViewModelFactory()
        {
        }

        public static PanelSectionViewModel CreateSidePanelSection(Point3D center, double xSectionSize, double ySectionSize, int i, int j, PanelSectionPosition position)
        {
            SidePanelSectionViewModel section = null;
            Dictionary<int, SidePanelSectionViewModel> dictonary = null;
            int index = -1;

            switch (position)
            {
                case PanelSectionPosition.SideBottom:
                    dictonary = _bottomSideSectionsDictionary;
                    index = i;
                    break;
                case PanelSectionPosition.SideTop:
                    dictonary = _topSideSectionsDictionary;
                    index = i;
                    break;
                case PanelSectionPosition.SideRight:
                    dictonary = _rightSideSectionsDictionary;
                    index = j;
                    break;
                case PanelSectionPosition.SideLeft:
                    dictonary = _leftSideSectionsDictionary;
                    index = j;
                    break;
                default:
                    throw new ArgumentException("Wrong panel position!");
            }

            if(dictonary.TryGetValue(index, out SidePanelSectionViewModel vm))
            {
                vm.XSectionIndex = i;
                vm.YSectionIndex = j;
                vm.SizeX = xSectionSize;
                vm.SizeY = ySectionSize;
                vm.SizeZ = SizeZ;
                vm.Center = center;

                vm.Reset();
                section = vm;
            }
            else
            {
                section = new SidePanelSectionViewModel()
                {
                    XSectionIndex = i,
                    YSectionIndex = j,
                    Position = position,
                    NumCells = NumCells,
                    SizeX = xSectionSize,
                    SizeY = ySectionSize,
                    SizeZ = SizeZ,
                    Center = center,
                    Visible = true
                };

                section.Initialize();
                dictonary.Add(index, section);
            }
                        
            return section;
        }

        public static PanelSectionViewModel CreateCornerPanelSection(Point3D center, double xSectionSize, double ySectionSize, int i, int j, PanelSectionPosition position)
        {
            CornerPanelSectionViewMoldel section = null;

            if(_cornerSectionsDictionary.TryGetValue(position, out CornerPanelSectionViewMoldel vm))
            {
                vm.XSectionIndex = i;
                vm.YSectionIndex = j;
                vm.SizeX = xSectionSize;
                vm.SizeY = ySectionSize;
                vm.SizeZ = SizeZ;
                vm.Center = center;

                vm.Reset();
                section = vm;
            }
            else
            {
                section = new CornerPanelSectionViewMoldel()
                {
                    XSectionIndex = i,
                    YSectionIndex = j,
                    Position = position,
                    NumCells = NumCells,
                    SizeX = xSectionSize,
                    SizeY = ySectionSize,
                    SizeZ = SizeZ,
                    Center = center,
                    Visible = true
                };

                section.Initialize();
                _cornerSectionsDictionary.Add(position, section);
            }
            
            return section;
        }


        public static PanelSectionViewModel CreateCenterPanelSection(Point3D center, double xSectionSize, double ySectionSize, int i, int j)
        {
            PanelSectionViewModel section = null;
            var idx = new SectionIndex();

            idx.SetIndex(i, j);

            if (_sectionsDictionary.TryGetValue(idx, out PanelSectionViewModel vm))
            {
                vm.SizeX = xSectionSize;
                vm.SizeY = ySectionSize;
                vm.SizeZ = SizeZ;
                vm.Center = center;

                vm.Reset();
                section = vm;
            }
            else
            {
                section = new PanelSectionViewModel()
                {
                    XSectionIndex = i,
                    YSectionIndex = j,
                    Position = PanelSectionPosition.Center,
                    NumCells = NumCells,
                    SizeX = xSectionSize,
                    SizeY = ySectionSize,
                    SizeZ = SizeZ,
                    Center = center,
                    Visible = true
                };

                section.Initialize();
                _sectionsDictionary.Add(idx, section);
            }

            return section;
        }

    }
}
