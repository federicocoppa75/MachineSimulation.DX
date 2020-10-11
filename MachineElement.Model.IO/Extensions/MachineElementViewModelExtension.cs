using MachineElements.ViewModels;
using System;
using System.Windows.Media.Media3D;
using MachineElements.ViewModels.Links;
using MachineElements.ViewModels.Links.Movement;
using MachineElements.ViewModels.Interfaces.Enums;
using LinkType = MachineElements.ViewModels.Interfaces.Enums.LinkType;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Extensions;

#if false
using LinearPositionViewModel = MachineElements.ViewModels.Links.Base.LinearPositionViewModel;
using RotaryPneumaticViewModel = MachineElements.ViewModels.Links.Base.RotaryPneumaticViewModel;
using LinearPneumaticViewModel = MachineElements.ViewModels.Links.Base.LinearPneumaticViewModel;
#else
using LinearPositionViewModel = MachineElements.ViewModels.Links.Evo.LinearPositionViewModel;
using RotaryPneumaticViewModel = MachineElements.ViewModels.Links.Evo.RotaryPneumaticViewModel;
using LinearPneumaticViewModel = MachineElements.ViewModels.Links.Evo.LinearPneumaticViewModel;
#endif

namespace MachineElement.Model.IO.Extensions
{
    public static class MachineElementViewModelExtension
    {
        public static void ApplyLinkAction(this MachineElementViewModel vm)
        {
            if (vm.LinkToParent != null)
            {
                switch (vm.LinkToParent.LinkType)
                {
                    case LinkType.LinearPosition:
                        ApplyLinearPositionLinkAction(vm);
                        break;
                    case LinkType.LinearPneumatic:
                        ApplyLinearPneumaticLinkAction(vm);
                        break;
                    case LinkType.RotaryPneumatic:
                        ApplyRotationPneumaticLinkAction(vm);
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
        }

        private static void ApplyLinearPositionLinkAction(IMachineElementViewModel vm)
        {
            var tg = new Transform3DGroup();

            tg.Children.Add(vm.Transform);

            var link = vm.LinkToParent as LinearPositionViewModel;
            var action = GetLinearPositionLinkAction(tg, vm.LinkToParent.Direction, link.Pos);

            link.ValueChanged += (s, e) => action(e);
            vm.Transform = tg;
        }

        private static Action<double> GetLinearPositionLinkAction(Transform3DGroup transformGroup, LinkDirection direction, double offset)
        {
            Action<double> action = null;
            var tt = new TranslateTransform3D();

            transformGroup.Children.Add(tt);

            switch (direction)
            {
                case LinkDirection.X:
                    action = (d) => tt.OffsetX = d - offset;
                    break;
                case LinkDirection.Y:
                    action = (d) => tt.OffsetY = d - offset;
                    break;
                case LinkDirection.Z:
                    action = (d) => tt.OffsetZ = d - offset;
                    break;
                default:
                    throw new ArgumentException("Invalid traslation direction!");
            }


            return action;
        }


        //private static void ApplyLinearPneumaticLinkAction(MachineElementViewModel vm)
        //{
        //    var tg = new Transform3DGroup();
        //    var tt = new TranslateTransform3D();
        //    var link = vm.LinkToParent as LinearPneumaticViewModel;

        //    tg.Children.Add(vm.Transform);
        //    tg.Children.Add(tt);

        //    var setAction = GetSetTraslationAction(link.Direction, tt);

        //    link.ValueChanged += (s, e) => setAction(e ? link.OnPos : link.OffPos);
        //    vm.Transform = tg;
        //}

        private static void ApplyLinearPneumaticLinkAction(IMachineElementViewModel vm)
        {
            var tg = new Transform3DGroup();
            var tt = new TranslateTransform3D();
            var link = vm.LinkToParent as LinearPneumaticViewModel;

            tg.Children.Add(vm.Transform);
            tg.Children.Add(tt);

            var setAction = GetSetTraslationAction(link.Direction, tt);

            link.SetPosition = setAction;
            link.GetPosition = GetGetTraslationFuncion(link.Direction, tt);

            link.ValueChanged += (s, e) =>
            {
                var onPos = link.OnPos;
                var offPos = link.OffPos;
                var tOn = link.TOn;
                var tOff = link.TOff;
                var toolActivator = link.ToolActivator;
                var lmevm = vm;
                var inserterId = link.InserterId;

                EvaluateLinkDataByCollision(link, e, ref onPos, offPos, ref tOn, ref tOff);

                if (link.IsGradualTransactionEnabled)
                {
                    var to = e ? onPos : offPos;
                    var t = e ? tOn : tOff;
                    link.OnMovementStarting?.Invoke(link);
                    link.SetPosition = (d) =>
                    {
                        setAction(d);
                        if (d == to)
                        {
                            link.OnMovementCompleted?.Invoke(link);
                            if (toolActivator) lmevm.ManageToolActivation(e);
                            if (inserterId > 0) link.ManageInserter(e);
                        }
                    };
                    LinearLinkMovementManager.Add(link.Id, link.Pos, to, t);
                }
                else
                {
                    var v = e ? onPos : offPos;
                    link.OnMovementStarting?.Invoke(link);
                    setAction(v);
                    link.OnMovementCompleted?.Invoke(link);
                    if (toolActivator) lmevm.ManageToolActivation(e);
                    if (inserterId > 0) link.ManageInserter(e);
                }
            };

            vm.Transform = tg;
        }

        private static Action<double> GetSetTraslationAction(LinkDirection direction, TranslateTransform3D tt)
        {
            Action<double> action = null;

            switch (direction)
            {
                case LinkDirection.X:
                    action = (d) => tt.OffsetX = d;
                    break;
                case LinkDirection.Y:
                    action = (d) => tt.OffsetY = d;
                    break;
                case LinkDirection.Z:
                    action = (d) => tt.OffsetZ = d;
                    break;
                default:
                    throw new ArgumentException("Invalid traslation direction!");
            }

            return action;
        }

        private static Func<double> GetGetTraslationFuncion(LinkDirection direction, TranslateTransform3D tt)
        {
            Func<double> function = null;

            switch (direction)
            {
                case LinkDirection.X:
                    function = () => tt.OffsetX;
                    break;
                case LinkDirection.Y:
                    function = () => tt.OffsetY;
                    break;
                case LinkDirection.Z:
                    function = () => tt.OffsetZ;
                    break;
                default:
                    throw new ArgumentException("Invalid traslation direction!");
            }

            return function;
        }


        private static void ApplyRotationPneumaticLinkAction(IMachineElementViewModel vm)
        {
            var rotVector = GetRotationDirection(vm.LinkToParent.Direction);
            var rotCenter = vm.Transform.Transform(new Point3D());
            var tg = new Transform3DGroup();
            var ar = new AxisAngleRotation3D(rotVector, 0.0);
            var tr = new RotateTransform3D(ar, rotCenter);
            var link = vm.LinkToParent as RotaryPneumaticViewModel;
            Action<double> setAction = (d) => ar.Angle = d;

            tg.Children.Add(vm.Transform);
            tg.Children.Add(tr);

            link.SetPosition = setAction;
            link.GetPosition = () => ar.Angle;

            link.ValueChanged += (s, e) =>
            {
                //setAction(e ? link.OnPos : link.OffPos);

                var onPos = link.OnPos;
                var offPos = link.OffPos;
                var tOn = link.TOn;
                var tOff = link.TOff;

                if (link.IsGradualTransactionEnabled)
                {
                    var to = e ? onPos : offPos;
                    var t = e ? tOn : tOff;
                    link.OnMovementStarting?.Invoke(link);
                    link.SetPosition = (d) =>
                    {
                        setAction(d);
                        if (d == to) link.OnMovementCompleted?.Invoke(link);
                    };
                    LinearLinkMovementManager.Add(link.Id, link.Pos, to, t);
                }
                else
                {
                    var v = e ? onPos : offPos;
                    link.OnMovementStarting?.Invoke(link);
                    setAction(v);
                    link.OnMovementCompleted?.Invoke(link);
                }

            };

            vm.Transform = tg;
        }

        private static Vector3D GetRotationDirection(LinkDirection direction)
        {
            Vector3D vector;
            switch (direction)
            {
                case LinkDirection.X: vector = new Vector3D(1.0, 0.0, 0.0); break;
                case LinkDirection.Y: vector = new Vector3D(0.0, 1.0, 0.0); break;
                case LinkDirection.Z: vector = new Vector3D(0.0, 0.0, 1.0); break;
                default: throw new ArgumentException("Invalid rotation direction!");
            }

            return vector;
        }

        private static void EvaluateLinkDataByCollision(IPneumaticColliderExtensionProvider link, bool b, ref double onPos, double offPos, ref double tOn, ref double tOff)
        {
            if (b && link.EvaluateCollision != null)
            {
                link.EvaluateCollision(link);
                if (link.HasCollision)
                {
                    var d1 = onPos - offPos;
                    var d2 = link.CollisionOnPos - offPos;

                    onPos = link.CollisionOnPos;
                    tOn *= d2 / d1;
                }
            }
            else if (!b && link.HasCollision && (link is IPneumaticPresserExtensionProvider ppp))
            {
                var d1 = onPos - offPos;
                var d2 = ppp.Pos - offPos;

                tOff *= d2 / d1;
            }
        }

    }
}
