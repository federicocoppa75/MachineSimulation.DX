using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WColor = System.Windows.Media.Color;
using Vector2 = SharpDX.Vector2;
using SXColor = SharpDX.Color;


namespace MachineElements.Views.Converters
{
    public class ColorToBitmapStreamConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if((values != null) && 
               (values.Length == 3) &&
               (values[0] is EffectsManager effectManager) &&
               (values[1] is WColor startColor) &&
               (values[2] is WColor stopColor))
            {
                var c1 = new SXColor(startColor.R, startColor.G, startColor.B, startColor.A);
                var c2 = new SXColor(stopColor.R, stopColor.G, stopColor.B, stopColor.A);
                var stream = BitmapExtensions.CreateLinearGradientBitmapStream(effectManager,
                                                                               128,
                                                                               128,
                                                                               Direct2DImageFormat.Bmp,
                                                                               new Vector2(0, 0),
                                                                               new Vector2(0, 128),
                                                                               new SharpDX.Direct2D1.GradientStop[]
                                                                               {
                                                                                   new SharpDX.Direct2D1.GradientStop(){ Color = c1, Position = 0f },
                                                                                   new SharpDX.Direct2D1.GradientStop(){ Color = c2, Position = 1f }
                                                                               });

                return new TextureModel(stream);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
