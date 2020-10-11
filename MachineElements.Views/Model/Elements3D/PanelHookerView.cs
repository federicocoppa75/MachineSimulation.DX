using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using MachineElements.ViewModels.Interfaces.Panel;
using MachineElements.Views.Messages.Models;
using System;
using Element3D = HelixToolkit.Wpf.SharpDX.Element3D;

namespace MachineElements.Views.Model.Elements3D
{
    public class PanelHookerView : Element3D
    {
        private Element3D _panelModel;

        public PanelHookerView() : base()
        {
            Messenger.Default.Register<ReturnPanelFromHookerMessage>(this, OnReturnPanelFromHookerMessage);
            Messenger.Default.Register<TransferPanelToHookerMessage>(this, OnTransferPanelToHookerMessage);
        }

        private void OnTransferPanelToHookerMessage(TransferPanelToHookerMessage msg)
        {
            if(ReferenceEquals(msg.PanelHooker, DataContext))
            {
                _panelModel = msg.TransferPanel();

                if (_panelModel != null)
                {
                    AddLogicalChild(_panelModel);
                    (SceneNode as GroupNode).AddChildNode(_panelModel.SceneNode);
                    _panelModel.Transform = (_panelModel.DataContext as IPanelViewModel).Transform;
                }
            }
        }   

        private void OnReturnPanelFromHookerMessage(ReturnPanelFromHookerMessage msg)
        {
            if(ReferenceEquals(msg.PanelHooker, DataContext))
            {
                if (_panelModel != null)
                {
                    RemoveLogicalChild(_panelModel);
                    (SceneNode as GroupNode).RemoveChildNode(_panelModel.SceneNode);
                    _panelModel.Transform = (_panelModel.DataContext as IPanelViewModel).Transform;
                    msg.ReturnPanel(_panelModel);
                }
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new GroupNode();
        }
    }
}
