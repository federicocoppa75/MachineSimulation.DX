using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MachineElements.Views.Converters
{
    public class BoolToVisibilityConverter : BoolToTypeConverter<Visibility>
    {
        public BoolToVisibilityConverter()
        {
            ValueForFalse = Visibility.Hidden;
            ValueForTrue = Visibility.Visible;
        }
    }
}
