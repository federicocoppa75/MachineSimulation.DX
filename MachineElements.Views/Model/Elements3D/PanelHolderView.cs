using HelixToolkit.Wpf.SharpDX.Model.Scene;
using System.Windows;
using Element3D = HelixToolkit.Wpf.SharpDX.Element3D;
using IPanelViewModel = MachineElements.ViewModels.Interfaces.Panel.IPanelViewModel;
using IPanelHooker = MachineElements.ViewModels.Colliders.IPanelHooker;
using GalaSoft.MvvmLight.Messaging;
using MachineElements.Views.Messages.Models;
using CompositeModel3D = HelixToolkit.Wpf.SharpDX.CompositeModel3D;

namespace MachineElements.Views.Model.Elements3D
{
    public class PanelHolderView : Element3D
    {
        private static object _lockObj;

        private Element3D _panelModel;

        //private IPanelViewModel _panelViewModel;
        public IPanelViewModel Panel
        {
            get { return (IPanelViewModel)GetValue(PanelProperty); }
            set { SetValue(PanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Panel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelProperty =
            DependencyProperty.Register("Panel", typeof(IPanelViewModel), typeof(PanelHolderView), new PropertyMetadata(null, (d, e) =>
            {
                var model = d as PanelHolderView;
                if (e.OldValue != null)
                {
                    model.RemoveLogicalChild(model._panelModel);
                    (model.SceneNode as GroupNode).RemoveChildNode(model._panelModel.SceneNode);

                    //foreach (var item in (model._panelModel as CompositeModel3D).Children)
                    //{
                    //    item.DataContext = null;
                    //    item.Dispose();
                    //}
                    //model._panelModel.DataContext = null;
                    //model._panelModel.Dispose();
                    //model._panelModel = null;
                    model.ResetPanelModel();
                }
                if (e.NewValue != null)
                {
                    var panelModel = model.PanelTemplate.LoadContent() as Element3D;
                    panelModel.DataContext = e.NewValue;
                    model.AddLogicalChild(panelModel);
                    (model.SceneNode as GroupNode).AddChildNode(panelModel.SceneNode);

                    model._panelModel = panelModel;
                }

                //model._panelViewModel = e.NewValue as IPanelViewModel;
            }));

        static PanelHolderView()
        {
            _lockObj = new object();
        }

        public DataTemplate PanelTemplate
        {
            get { return (DataTemplate)GetValue(PanelTemplateProperty); }
            set { SetValue(PanelTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelTemplateProperty =
            DependencyProperty.Register("PanelTemplate", typeof(DataTemplate), typeof(PanelHolderView), new PropertyMetadata(null));


        public IPanelHooker PanelHoker
        {
            get { return (IPanelHooker)GetValue(PanelHokerProperty); }
            set { SetValue(PanelHokerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelHoker.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelHokerProperty =
            DependencyProperty.Register("PanelHoker", typeof(IPanelHooker), typeof(PanelHolderView), new PropertyMetadata(null, (d, e) =>
            {
                lock (_lockObj)
                {
                    var model = d as PanelHolderView;
                    if (e.OldValue != null)
                    {
                        Messenger.Default.Send(new ReturnPanelFromHookerMessage()
                        {
                            PanelHooker = e.OldValue,
                            ReturnPanel = (ele) =>
                            {
                                if (model._panelModel != null)
                                {
                                    model.AddLogicalChild(model._panelModel);
                                    (model.SceneNode as GroupNode).AddChildNode(model._panelModel.SceneNode);
                                }
                            }
                        });
                    }
                    if (e.NewValue != null)
                    {
                        Messenger.Default.Send(new TransferPanelToHookerMessage()
                        {
                            PanelHooker = e.NewValue,
                            TransferPanel = () =>
                            {
                                if (model._panelModel != null)
                                {
                                    model.RemoveLogicalChild(model._panelModel);
                                    (model.SceneNode as GroupNode).RemoveChildNode(model._panelModel.SceneNode);
                                }
                                return model._panelModel;
                            }
                        });
                    }
                }
            }));


        public PanelHolderView() : base()
        {
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new GroupNode();
        }

        private void ResetPanelModel()
        {
            foreach (var item in (_panelModel as CompositeModel3D).Children)
            {
                item.DataContext = null;
                item.Dispose();
            }
            _panelModel.DataContext = null;
            _panelModel.Dispose();
            _panelModel = null;
        }
    }
}
