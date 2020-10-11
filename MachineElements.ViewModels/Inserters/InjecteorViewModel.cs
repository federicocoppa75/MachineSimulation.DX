using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Extensions;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Messages.Generic;
using MachineElements.ViewModels.Messages.Inserters;
using MachineElements.ViewModels.Messages.Links;
using MachineElements.ViewModels.Messages.Panel;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using InjectMessage = MachineElements.ViewModels.Interfaces.Messages.Panel.InjectMessage;

namespace MachineElements.ViewModels.Inserters
{
    public class InjectorViewModel : InserterBaseViewModel
    {
        [Browsable(false)]
        public bool IsGradualTransactionEnabled { get; set; }


        public InjectorViewModel() : base()
        {
            Messenger.Default.Register<GetAvailablaInjectorsMessage>(this, OnGetAvailablaInjectorsMessage);
            Messenger.Default.Register<ExecuteInjectionMessage>(this, OnExecuteInjectionMessage);
            Messenger.Default.Register<InjectActionMessage>(this, OnInjectMessage);
            Messenger.Default.Register<EnableGradualTransitionMessage>(this, OnEnableGradualTransitionMessage);
        }

        private void OnGetAvailablaInjectorsMessage(GetAvailablaInjectorsMessage msg) => msg?.SetInjectorData(InserterId);

        private void OnExecuteInjectionMessage(ExecuteInjectionMessage msg)
        {
            if(msg.Id == InserterId)
            {
                ExecuteInjection();
            }
        }

        private void OnInjectMessage(InjectActionMessage msg)
        {
            if (msg.InjectorId == InserterId)
            {
                ExecuteInjection();

                if (msg.BackNotifyId > 0)
                {
                    if (IsGradualTransactionEnabled)
                    {
                        Task.Delay(TimeSpan.FromSeconds(msg.Duration))
                            .ContinueWith((t) =>
                            {
                                Messenger.Default.Send(new BackNotificationMessage() { DestinationId = msg.BackNotifyId });
                            });
                    }
                    else
                    {
                        Messenger.Default.Send(new BackNotificationMessage() { DestinationId = msg.BackNotifyId });
                    }
                }                
            }
        }

        private void OnEnableGradualTransitionMessage(EnableGradualTransitionMessage msg) => IsGradualTransactionEnabled = msg.Value;

        private void ExecuteInjection()
        {
            if (IsBackStepProgress) return;
            if (_chainTransform == null) _chainTransform = this.GetChainTansform();

            var t = _chainTransform.Value;
            var p = t.Transform(Position);
            var d = t.Transform(Direction);

            Messenger.Default.Send(new GetPanelTransformMessage()
            {
                SetData = (b, m) =>
                {
                    if (b)
                    {
                        var invT = m;

                        invT.Invert();

                        Messenger.Default.Send(new InjectMessage()
                        {
                            InjectElement = CreateInjectElement(invT.Transform(p), invT.Transform(d))
                        });
                    }
                }
            });
        }

        private IMachineElementViewModel CreateInjectElement(Point3D position, Vector3D direction)
        {
            var builder = new HelixToolkit.Wpf.SharpDX.MeshBuilder();

            builder.AddCone((position + direction * 20.0).ToVector3(),
                            position.ToVector3(),
                            4.0,
                            true,
                            20);
            return new InserterObjectViewModel() { Geometry = builder.ToMesh(), Material = Material, Visible = true };
        }
    }
}
