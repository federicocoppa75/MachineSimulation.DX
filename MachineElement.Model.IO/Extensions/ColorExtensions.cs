using HelixToolkit.Wpf.SharpDX;
using MachineModels.Models;

namespace MachineElement.Model.IO.Extensions
{
    public static class ColorExtensions
    {
        public static Material ToMaterial(this Color color)
        {
            var material = new PhongMaterial();

            material.AmbientColor = new SharpDX.Color4(1.0f);
            material.DiffuseColor = new SharpDX.Color4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
            material.SpecularColor = new SharpDX.Color4(1.0f);
            material.SpecularShininess = 100.0f;

            return material;
        }
    }
}
