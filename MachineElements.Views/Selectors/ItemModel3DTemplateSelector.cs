using MachineElements.Views.Enums;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using PanelHolderViewModel = MachineElements.ViewModels.Panel.PanelHolderViewModel;
using IPanelHooker = MachineElements.ViewModels.Colliders.IPanelHooker;
using IPanelViewModel = MachineElements.ViewModels.Interfaces.Panel.IPanelViewModel;
using PointsDistanceViewModel = MachineElements.ViewModels.Probing.PointsDistanceViewModel;

namespace MachineElements.Views.Selectors
{
    [ContentProperty("Templates")]
    public class ItemModel3DTemplateSelector : DataTemplateSelector
    {
        public List<ItemModel3DTemplateSelectorItem> Templates { get; set; } = new List<ItemModel3DTemplateSelectorItem>();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate dt = null;
            var it = GetItemType(item);

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

        private static ElementViewType GetItemType(object item)
        {
            var it = ElementViewType.Default;

            //if ((item is IPanelHooker) || (item is PanelHolderViewModel))
            //{
            //    it = ElementViewType.PanelHandler;
            //}
            //else if (item is IPanelViewModel)
            //{
            //    it = ElementViewType.Panel;
            //}

            if(item is PanelHolderViewModel)
            {
                it = ElementViewType.PanelHolder;
            }
            else if(item is IPanelHooker)
            {
                it = ElementViewType.PanelHooker;
            }
            else if(item is PointsDistanceViewModel)
            {
                it = ElementViewType.PointDistance;
            }

            return it;
        }
    }

    [ContentProperty("Then")]
    public class ItemModel3DTemplateSelectorItem
    {
        public ElementViewType When { get; set; }
        public DataTemplate Then { get; set; }
    }
}
