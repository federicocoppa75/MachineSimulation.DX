using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels;
using Models = MachineModels.Models;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Matrix3D = System.Windows.Media.Media3D.Matrix3D;
using Matrix3DTransform = System.Windows.Media.Media3D.MatrixTransform3D;
using MachineElements.ViewModels.Links;
using MachineModels.Enums;
using MachineModels.Models.Links;
using System;
using MachineElement.Model.IO.Extensions;
using MachineElements.ViewModels.ToolHolder;
using MachineElements.ViewModels.Interfaces;
using MachineModels.IO;

namespace MachineElement.Model.IO
{
    public static class MachineLoader
    {
        public static MachineElementViewModel LoadMachineFromFile(string machProjectFile)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Models.MachineElement));

            using (var reader = new System.IO.StreamReader(machProjectFile))
            {
                var me = (Models.MachineElement)serializer.Deserialize(reader);
                var vm = ConvertModelToViewModel(me);

                return vm;
            }
        }

        public static MachineElementViewModel LoadMachineFromArchive(string machineFile)
        {
            var m = ZipArchiveHelper.Import(machineFile/*, (s) => _lastMachProjectFile = s*/);
            return ConvertModelToViewModel(m);
        }

        public static bool ImportEnvironment(string fileName, out string machProjectFile, out string toolsFile, out string toolingFile)
        {
            return ZipArchiveHelper.ImportEnvironment(fileName, out machProjectFile, out toolsFile, out toolingFile);
        }

        public static void ExportEnvironment(string fileName, string machProjectFile, string toolsFile, string toolingFile)
        {
            ZipArchiveHelper.ExportEnvironment(fileName, machProjectFile, toolsFile, toolingFile);
        }

        private static MachineElementViewModel ConvertModelToViewModel(Models.MachineElement me, IMachineElementViewModel parent = null)
        {
            var vm = CreateViewModel(me);

            UpdateFromModel(me, vm);
            vm.Parent = parent;

            foreach (var item in me.Children)
            {
                var childVm = ConvertModelToViewModel(item, vm);

                vm.Children.Add(childVm);
            }

            if (me.HasPanelHolder) vm.Children.Add(me.PanelHolder.ToViewModel(vm));
            if (me.InserterType != InserterType.None) vm.Children.Add(me.Inserter.ToViewModel(vm));
            if (me.ColiderType != ColliderGeometry.None) vm.Children.Add(me.Collider.ToViewModel(vm));

            return vm;
        }

        private static void UpdateFromModel(Models.MachineElement me, MachineElementViewModel vm)
        {
            vm.Name = me.Name;
            vm.Geometry = LoadGeometry(me.ModelFile);
            vm.Transform = ConvertTransform(me.TrasformationMatrix3D);
            vm.Material = me.Color.ToMaterial();
            vm.LinkToParent = me.LinkToParentData.Convert();

            if (vm.LinkToParent != null) vm.LinkToParent.Description = vm.Name;
            vm.ApplyLinkAction();
        }

        private static MachineElementViewModel CreateViewModel(Models.MachineElement me)
        {
            MachineElementViewModel vm;

            switch (me.ToolHolderType)
            {
                case MachineModels.Enums.ToolHolderType.None:
                    vm = new MachineElementViewModel();
                    break;
                case MachineModels.Enums.ToolHolderType.Static:
                    vm = new StaticToolHolderViewModel().UpdateFromModel(me.ToolHolderData);
                    break;
                case MachineModels.Enums.ToolHolderType.AutoSource:
                    vm = new AutoSourceToolHolderViewModel().UpdateFromModel(me.ToolHolderData);
                    break;
                case MachineModels.Enums.ToolHolderType.AutoSink:
                    vm = new AutoSinkToolHolderViewModel().UpdateFromModel(me.ToolHolderData);
                    break;
                default:
                    throw new NotImplementedException();
            }

            vm.Visible = true;

            return vm;
        }


        private static Transform3D ConvertTransform(Models.Matrix3D matrix3D)
        {
            var m3d = new Matrix3D(matrix3D.M11, matrix3D.M12, matrix3D.M13, matrix3D.M14,
                                   matrix3D.M21, matrix3D.M22, matrix3D.M23, matrix3D.M24,
                                   matrix3D.M31, matrix3D.M32, matrix3D.M33, matrix3D.M34,
                                   matrix3D.OffsetX, matrix3D.OffsetY, matrix3D.OffsetZ, matrix3D.M44);
            var mt = new Matrix3DTransform(m3d);

            return mt;
        }

        private static Geometry3D LoadGeometry(string geometryFile)
        {
            Geometry3D geometry = null;

            if (!string.IsNullOrEmpty(geometryFile))
            {
                var reader = new StLReader();
                var objList = reader.Read(geometryFile);

                if(objList?.Count > 0)
                {
                    geometry = objList[0].Geometry;
                    geometry.UpdateOctree();
                }
            }

            return geometry;
        }
    }
}
