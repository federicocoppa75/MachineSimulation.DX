using CncViewer.Models.Connection.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CncViewer.ConfigEditor.Converters
{
    public class ChannelTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((value is ChannelType v) && (parameter is ChannelType p))
            {
                return v == p;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is bool v) && (parameter is ChannelType p) && v)
            {
                return parameter;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
