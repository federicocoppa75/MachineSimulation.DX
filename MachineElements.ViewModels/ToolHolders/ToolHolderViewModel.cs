using GalaSoft.MvvmLight.Messaging;
using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Extensions;
using MachineElements.ViewModels.Helpers;
using MachineElements.ViewModels.Helpers.UI;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Messages.Tooling;
using MachineElements.ViewModels.Messages.ToolState;
using MachineElements.ViewModels.Tools;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Tools.Models;
using TME = Tools.Models.Enums;

namespace MachineElements.ViewModels.ToolHolder
{
    public abstract class ToolHolderViewModel : MachineElementViewModel
    {
        private Transform3D _chainTransform;

        protected Tool _tool;

        public abstract ToolHolderType ToolHolderType { get; }

        public int ToolHolderId { get; set; }

        public Point3D Position { get; set; }

        public Vector3D Direction { get; set; }

        public string ToolName { get; set; } = string.Empty;

        public bool ActiveTool { get; set; }

        public ToolHolderViewModel() : base()
        {
            Messenger.Default.Register<LoadToolMessage>(this, OnLoadTool);
            Messenger.Default.Register<UnloadToolMessage>(this, OnUnloadTool);
            Messenger.Default.Register<GetActiveToolMessage>(this, OnGetActiveToolMessage);
            Messenger.Default.Register<GetActiveRoutToolMessage>(this, OnGetActiveRoutToolMessage);
        }

        private void OnUnloadTool(UnloadToolMessage msg)
        {
            Children.Clear();
            ToolName = string.Empty;
            _tool = null;
        }

        private void OnLoadTool(LoadToolMessage msg)
        {
            if (msg.ToolHolderId == ToolHolderId)
            {
                Children.Add(GetToolViewModel(msg.Tool));
                ToolName = msg.Tool.Name;
                _tool = msg.Tool;
            }
        }

        private IMachineElementViewModel GetToolViewModel(Tool tool)
        {
            IMachineElementViewModel vm = null;

            if(tool.ToolType == TME.ToolType.AngularTransmission)
            {
                vm = AngularTransmissionViewModel.Create(tool, Position, Direction);
            }
            else if(!string.IsNullOrEmpty(tool.ConeModelFile))
            {
                vm = ToolWithConeViewModel.Create(tool, Position, Direction);
            }
            else
            {
                vm = ToolViewModel.Create(tool, Position, Direction);
            }

            vm.Parent = this;

            return vm;
        }


        private void OnGetActiveToolMessage(GetActiveToolMessage msg)
        {
            if (ActiveTool)
            {
                DispatcherHelperEx.CheckBeginInvokeOnUI(() =>
                {
                    var t = this.GetChainTansform();
                    var p = t.Transform(Position);
                    var v = t.Transform(Direction);
                    msg.SetData(p, v, _tool);
                });
            }
        }

        private void OnGetActiveRoutToolMessage(GetActiveRoutToolMessage msg)
        {
            if (ActiveTool && (_tool != null))
            {
                DispatcherHelperEx.CheckBeginInvokeOnUI(() =>
                {
                    if (_chainTransform == null) _chainTransform = this.GetChainTansform();

                    var t = _chainTransform.Value;

                    Task.Run(() =>
                    {
                        if(_tool.ToolType == TME.ToolType.AngularTransmission)
                        {
                            var at = _tool as AngolarTransmission;

                            foreach (var item in at.Subspindles)
                            {
                                var p = t.Transform(Position + item.Position.ToVector3D());
                                var v = t.Transform(item.Direction.ToVector3D());
                                msg.SetData(p, v, item.Tool, ToolHolderId);
                            }
                        }
                        else
                        {
                            var p = t.Transform(Position);
                            var v = t.Transform(Direction);
                            msg.SetData(p, v, _tool, ToolHolderId);
                        }
                    });
                });
            }
        }
    }
}
