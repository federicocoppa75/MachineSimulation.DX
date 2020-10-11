using HelixToolkit.Wpf.SharpDX.Elements2D;
using HelixToolkit.Wpf.SharpDX.Model.Scene2D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

namespace MachineElements.Views.Model.Elements2D
{
    public class ItemsModel2D : Panel2D
    {
        protected readonly Dictionary<object, Element2D> elementDict = new Dictionary<object, Element2D>();

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ItemsModel2D), 
                                        new PropertyMetadata(null, (s, e) => ((ItemsModel2D)s).ItemsSourceChanged(e)));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ItemsModel2D), new PropertyMetadata(null));


        public ItemsModel2D() : base()
        {
        }

        private void ItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged o)
            {
                o.CollectionChanged -= ItemSourceCollectionChanged;
            }

            Clear();

            if (e.NewValue is INotifyCollectionChanged n)
            {
                n.CollectionChanged -= ItemSourceCollectionChanged;
                n.CollectionChanged += ItemSourceCollectionChanged;
            }

            if (ItemsSource == null) return;

            if(this.ItemTemplate == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                AddChildrenByItemsSource();
            }
        }

        private void ItemSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (elementDict.TryGetValue(item, out Element2D element))
                            {
                                Children.Remove(element);
                                elementDict.Remove(item);
                            }
                        }
                        InvalidateRender();
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    break;
                default:
                    break;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    if (this.ItemsSource != null)
                    {
                        if(this.ItemTemplate != null)
                        {
                            AddChildrenByItemsSource();
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    InvalidateRender();
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    if(e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            AddChildByDataContext(item);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void AddChildrenByItemsSource()
        {
            foreach (var item in this.ItemsSource)
            {
                AddChildByDataContext(item);
            }
        }

        private void AddChildByDataContext(object dataContect)
        {
            var model = this.ItemTemplate.LoadContent() as Element2D;

            if (model != null)
            {
                model.DataContext = dataContect;
                this.Children.Add(model);
                elementDict.Add(dataContect, model);
            }
            else
            {
                throw new InvalidOperationException("Cannot create a Model2D from ItemTemplate.");
            }
        }

        protected override SceneNode2D OnCreateSceneNode()
        {
            return new StackPanelNode2D() { Orientation = Orientation.Vertical };
        }

        public void Clear()
        {
            elementDict.Clear();

            foreach (Element2D item in Children)
            {
                item.DataContext = null;
                item.Dispose();
            }

            Children.Clear();
        }
    }
}
