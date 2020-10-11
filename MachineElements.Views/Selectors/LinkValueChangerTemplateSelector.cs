using MachineElements.ViewModels.Interfaces.Enums;
using MachineElements.ViewModels.Interfaces.Links;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MachineElements.Views.Selectors
{
    [ContentProperty("Templates")]
    public class LinkValueChangerTemplateSelector : DataTemplateSelector
    {

        public List<LinkValueChangerTemplateSelectorOptions> Templates { get; set; } = new List<LinkValueChangerTemplateSelectorOptions>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dt = null;

            if (item is ILinkViewModel link)
            {
                foreach (var t in Templates)
                {
                    if (t.When == link.LinkType)
                    {
                        dt = t.Then;
                        break;
                    }
                }
            }

            return dt;
        }
    }

    [ContentProperty("Then")]
    public class LinkValueChangerTemplateSelectorOptions
    {
        public LinkType When { get; set; }
        public DataTemplate Then { get; set; }
    }
}
