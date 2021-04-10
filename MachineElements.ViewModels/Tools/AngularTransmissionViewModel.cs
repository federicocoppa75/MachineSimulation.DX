using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;
using Tools.Models;

namespace MachineElements.ViewModels.Tools
{
    public class AngularTransmissionViewModel : MachineElementViewModel
    {
        public AngularTransmissionViewModel() : base()
        {
        }

        public static AngularTransmissionViewModel Create(Tool tool, Point3D position, Vector3D direction)
        {
            var at = tool as AngolarTransmission;
            var atvm = new AngularTransmissionViewModel()
            {
                Name = at.Name,
                Material = PhongMaterials.PolishedSilver,
                Geometry = ToolsHelpers.GetConeModel(at.BodyModelFile),
                Transform = new TranslateTransform3D() { OffsetX = position.X, OffsetY = position.Y, OffsetZ = position.Z },
                Visible = true
            };

            foreach (var item in at.Subspindles)
            {
                atvm.Children.Add(ToolViewModel.Create(item.Tool, item.Position.ToPoint3D(), item.Direction.ToVector3D()));
            }

            return atvm;
        }
    }
}
