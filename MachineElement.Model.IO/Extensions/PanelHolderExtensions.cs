using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Panel;
using MachineModels.Models;
using MachineModels.Models.PanelHolders;
using System;
using System.Windows.Media.Media3D;
using MPanelLoadType = MachineModels.Enums.PanelLoadType;
using VMPanelLoadType = MachineElements.ViewModels.Enums.PanelLoadType;

namespace MachineElement.Model.IO.Extensions
{
    public static class PanelHolderExtensions
    {
        public static PanelHolderViewModel ToViewModel(this PanelHolder ph, IMachineElementViewModel parent = null)
        {
            return new PanelHolderViewModel()
            {
                PanelHolderId = ph.Id,
                Name = ph.Name,
                Corner = Convert(ph.Corner),
                Position = Convert(ph.Position),
                Transform = new TranslateTransform3D(ph.Position.X, ph.Position.Y, ph.Position.Z),
                Geometry = CreateGeometry(),
                Material = PhongMaterials.Blue,
                Parent = parent
            };
        }

        private static HelixToolkit.Wpf.SharpDX.Geometry3D CreateGeometry()
        {
            var builder = new MeshBuilder();

            builder.AddSphere(new SharpDX.Vector3(), 10.0);

            return builder.ToMesh();
        }

        private static VMPanelLoadType Convert(MPanelLoadType type)
        {
            switch (type)
            {
                case MPanelLoadType.Corner1:
                    return VMPanelLoadType.Corner1;
                case MPanelLoadType.Corner2:
                    return VMPanelLoadType.Corner2;
                case MPanelLoadType.Corner3:
                    return VMPanelLoadType.Corner3;
                case MPanelLoadType.Corner4:
                    return VMPanelLoadType.Corner4;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Point3D Convert(Vector position) => new Point3D(position.X, position.Y, position.Z);
    }
}
