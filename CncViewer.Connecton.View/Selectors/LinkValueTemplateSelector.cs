using CncViewer.Connection.Enums;
using CncViewer.Connection.ViewModels.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CncViewer.Connecton.View.Selectors
{
    [ContentProperty("Templates")]
    class LinkValueTemplateSelector : DataTemplateSelector
    {
        public List<LinkValueTemplateSelectorItem> Templates { get; set; } = new List<LinkValueTemplateSelectorItem>();

        
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dt = null;
            var it = (item as LinkViewModel).Type;

            foreach (var t in Templates)
            {
                if (t.When == it)
                {
                    dt = t.Then;
                    break;
                }
            }

            return dt;
        }

    }

    [ContentProperty("Then")]
    class LinkValueTemplateSelectorItem
    {
        public LinkType When { get; set; }
        public DataTemplate Then { get; set; }
    }
}
