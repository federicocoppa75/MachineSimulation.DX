using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels;
using MachineElements.ViewModels.Inserters;
using MachineModels.Models.Inserters;

namespace MachineElement.Model.IO.Extensions
{
    public static class InserterExtensions
    {
        public static InserterBaseViewModel ToViewModel(this BaseInserter m, MachineElementViewModel parent = null)
        {
            InserterBaseViewModel vm = null;

            if (m is Inserter ins)
            {
                var insVm = new InserterViewModel()
                {
                    Length = ins.Length,
                    Diameter = ins.Diameter,
                    LoaderLinkId = ins.LoaderLinkId,
                    DischargerLinkId = ins.DischargerLinkId
                };

                vm = insVm;
            }
            else if (m is Injector inj)
            {
                vm = new InjectorViewModel();
            }

            vm?.UpdateFrom(m);
            if (vm != null && parent != null) vm.Parent = parent;

            return vm;
        }

        public static void UpdateFrom(this InserterBaseViewModel vm, BaseInserter m)
        {
            vm.InserterId = m.Id;
            vm.Position = new System.Windows.Media.Media3D.Point3D(m.Position.X, m.Position.Y, m.Position.Z);
            vm.Direction = new System.Windows.Media.Media3D.Vector3D(m.Direction.X, m.Direction.Y, m.Direction.Z);
            var material = PhongMaterials.Glass;
            material.DiffuseColor = PhongMaterials.ToColor(m.Color.R/255.0, m.Color.G/255.0, m.Color.B/255.0, 1.0);
            vm.Material = material;
        }

        public static System.Windows.Media.Color Convert(this MachineModels.Models.Color c)
        {
            return new System.Windows.Media.Color()
            {
                A = c.A,
                R = c.R,
                G = c.G,
                B = c.B
            };
        }
    }
}
