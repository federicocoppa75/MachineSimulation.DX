using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Messages.Generic;
using MachineElements.ViewModels.Messages.ToolChange;
using System;
using Transform3D = System.Windows.Media.Media3D.Transform3D;

namespace MachineElements.ViewModels.ToolHolder
{
    public class AutoSourceToolHolderViewModel : ToolHolderViewModel
    {
        // modello dell'utensile prima del caricamento verso un sink (ovvero in partenza da questo tool holder)
        private IMachineElementViewModel _loadedTool;

        // trasformazione originale prima del caricamento verso un sink (ovvero in partenza da questo tool holder)
        private Transform3D _preLoadTransform;

        public override ToolHolderType ToolHolderType => ToolHolderType.AutoSource;

        public AutoSourceToolHolderViewModel() : base()
        {
            MessengerInstance.Register<GetAvailableToolMessage>(this, OnGetAvailableToolMessage);
            MessengerInstance.Register<LoadToolMessage>(this, OnLoadToolMessage);
            MessengerInstance.Register<UnloadToolMessage>(this, OnUnloadToolMessage);
            MessengerInstance.Register<UnloadAllToolsMessage>(this, OnUnloadAllToolsMessage);
        }

        private void OnGetAvailableToolMessage(GetAvailableToolMessage msg)
        {
            if (!string.IsNullOrEmpty(ToolName)) msg?.SetAvailableTool(ToolHolderId, ToolName);
        }

        private void OnLoadToolMessage(LoadToolMessage msg)
        {
            if (msg.ToolSource == ToolHolderId)
            {
                if (Children.Count == 1)
                {
                    _loadedTool = Children[0];
                    _preLoadTransform = _loadedTool.Transform;
                    Children.Clear();
                    MessengerInstance.Send(new CompleteToolLoadingMessage()
                    {
                        ToolSink = msg.ToolSink,
                        ToolModel = _loadedTool,
                        ToolData = _tool,
                        BackNotifyId = msg.BackNotifyId
                    });
                }
                else
                {
                    throw new InvalidOperationException("Load tool from empty tool holder source!");
                }
            }
        }

        private void OnUnloadToolMessage(UnloadToolMessage msg)
        {
            if (msg.ToolSource == ToolHolderId)
            {
                ApplyUnloadTool();
                if (msg.BackNotifyId > 0) MessengerInstance.Send(new BackNotificationMessage() { DestinationId = msg.BackNotifyId });
            }
        }

        private void OnUnloadAllToolsMessage(UnloadAllToolsMessage msg)
        {
            ApplyUnloadTool();
        }


        private void ApplyUnloadTool()
        {
            if ((_loadedTool != null) /*&& (_preLoadTransform != null)*/)
            {
                _loadedTool.Transform = _preLoadTransform;
                Children.Add(_loadedTool);
                _loadedTool = null;
                _preLoadTransform = null;
            }
        }

    }
}
