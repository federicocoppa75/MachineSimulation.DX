using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Messages.Generic;
using MachineElements.ViewModels.Messages.ToolChange;
using System;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;

namespace MachineElements.ViewModels.ToolHolder
{
    public class AutoSinkToolHolderViewModel : ToolHolderViewModel
    {
        public override ToolHolderType ToolHolderType => ToolHolderType.AutoSink;

        public AutoSinkToolHolderViewModel() : base()
        {
            MessengerInstance.Register<GetAvailableToolSinkMessage>(this, OnGetAvailableToolSinkMessage);
            MessengerInstance.Register<CompleteToolLoadingMessage>(this, OnCompleteToolLoadingMessage);
            MessengerInstance.Register<UnloadToolMessage>(this, OnUnloadToolMessage);
            MessengerInstance.Register<UnloadAllToolsMessage>(this, OnUnloadAllToolsMessage);
        }

        private void OnGetAvailableToolSinkMessage(GetAvailableToolSinkMessage msg)
        {
            msg?.SetAvailableToolSink(ToolHolderId, Name);
        }

        private void OnCompleteToolLoadingMessage(CompleteToolLoadingMessage msg)
        {
            if (msg.ToolSink == ToolHolderId)
            {
                if (Children.Count == 0)
                {
                    msg.ToolModel.Transform = new TranslateTransform3D() { OffsetX = Position.X, OffsetY = Position.Y, OffsetZ = Position.Z };
                    Children.Add(msg.ToolModel);
                    _tool = msg.ToolData;
                    if (msg.BackNotifyId > 0) MessengerInstance.Send(new BackNotificationMessage() { DestinationId = msg.BackNotifyId });
                }
                else
                {
                    throw new InvalidOperationException("Could not load tool in full tool holder!");
                }
            }
        }

        private void OnUnloadToolMessage(UnloadToolMessage msg)
        {
            if (msg.ToolSink == ToolHolderId)
            {
                _tool = null;
                Children.Clear();
            }
        }

        private void OnUnloadAllToolsMessage(UnloadAllToolsMessage msg)
        {
            Children.Clear();
        }

    }
}
