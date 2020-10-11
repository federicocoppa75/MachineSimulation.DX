using CncViewer.Connection.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace CncViewer.Connecton.View.Converters
{
    [ContentProperty("Values")]
    class VariableTypeToImageConverter : IValueConverter
    {
        public List<VariableTypeToImageConverterItem> Values { get; set; } = new List<VariableTypeToImageConverterItem>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = null;

            if (value is LinkType pt)
            {
                foreach (var item in Values)
                {
                    if (item.When == pt)
                    {
                        result = item.Then;
                        break;
                    }
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ContentProperty("Then")]
    class VariableTypeToImageConverterItem
    {
        public LinkType When { get; set; }
        public object Then { get; set; }
    }
}
