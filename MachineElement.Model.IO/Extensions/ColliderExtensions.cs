using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels;
using MachineElements.ViewModels.Colliders;
using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Interfaces.Enums;
using MachineElements.ViewModels.Links;
using MachineElements.ViewModels.Links.Base;
using MachineModels.Models.Colliders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace MachineElement.Model.IO.Extensions
{
    public static class ColliderExtensions
    {
        public static MachineElementViewModel ToViewModel(this Collider m, MachineElementViewModel parent = null)
        {
            MachineElementViewModel vm = null;

            if (m is PointsCollider pcm)
            {
                switch (pcm.Type)
                {
                    case MachineModels.Enums.ColliderType.Presser:
                        {
                            var pcvm = new PointsColliderViewModel()
                            {
                                Type = ConvertColliderType(pcm.Type),
                                Radius = pcm.Radius,
                                Points = pcm.Points.Select((p) => p.ToPoint3D()).ToList(),
                                Parent = parent,
                                Material = PhongMaterials.Green
                            };

                            pcvm.Geometry = GetMeshGeometry(pcvm.Radius, pcvm.Points);

                            AttachProbeToLinkForPresser(pcvm);

                            vm = pcvm;
                        }
                        break;

                    case MachineModels.Enums.ColliderType.Gripper:
                        {
                            var pcvm = new PointsColliderViewModel()
                            {
                                Type = ConvertColliderType(pcm.Type),
                                Radius = pcm.Radius,
                                Points = pcm.Points.Select((p) => p.ToPoint3D()).ToList(),
                                Parent = parent,
                                Material = PhongMaterials.Red
                            };

                            pcvm.Geometry = GetMeshGeometry(pcvm.Radius, pcvm.Points);

                            AttachProbeToLink(pcvm);

                            vm = pcvm;
                        }
                        break;

                    case MachineModels.Enums.ColliderType.Detect:
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                throw new ArgumentException("Unexpected collider type!");
            }

            return vm;
        }

        private static ColliderType ConvertColliderType(MachineModels.Enums.ColliderType type)
        {
            switch (type)
            {
                case MachineModels.Enums.ColliderType.Detect:
                    return ColliderType.Detect;
                case MachineModels.Enums.ColliderType.Presser:
                    return ColliderType.Presser;
                case MachineModels.Enums.ColliderType.Gripper:
                    return ColliderType.Gripper;
                default:
                    throw new ArgumentException("Invalid collider type!");
            }
        }

        private static void AttachProbeToLinkForPresser(ColliderViewModel vm)
        {
            var apvm = GetFirstAncestorWithPneumaticLink(vm);

            if (apvm != null)
            {
                var pnLink = apvm.LinkToParent as IPneumaticPresserExtensionProvider;
                var lpvm = GetFirstAncestorWithLinearPositionLink(vm, pnLink.Direction);

                if(lpvm != null)
                {
                    var lpLink = lpvm.LinkToParent as LinearPositionViewModel;
                    var sign = Math.Sign(pnLink.OnPos - pnLink.OffPos);
                    var direction = GetDirectionVector(pnLink.Direction) * sign;

                    AttachActionToCollider(vm, pnLink);

                    lpLink.ValueChanged += (s, e) =>
                    {
                        var t = vm.CheckPanelIntersectionAsync(pnLink.Value, direction).Result;
                        //var t = vm.CheckPanelIntersection(pnLink.Value, direction);
                        
                        if (t.Item1)
                        {
                            double intersectValue = t.Item2;

                            if (intersectValue <= 0)
                            {
                                pnLink.HasCollision = true;
                                vm.Collided = true;
                                pnLink.Pos = pnLink.Pos - intersectValue;
                            }
                            else if(vm.Collided)
                            {
                                pnLink.Pos = pnLink.Pos - intersectValue;

                                //verifico se il link si sia esteso completamente
                                bool b = (sign > 0) ? (pnLink.Pos > pnLink.OnPos) : (pnLink.Pos < pnLink.OnPos);

                                if (b)
                                {
                                    pnLink.HasCollision = false;
                                    vm.Collided = false;
                                    pnLink.Pos = pnLink.OnPos;
                                }
                            }
                        }
                    };
                } 
                else
                {
                    AttachActionToCollider(vm, pnLink);
                }
            }
        }

        private static void AttachProbeToLink(ColliderViewModel vm)
        {
            var avm = GetFirstAncestorWithPneumaticLink(vm);

            if (avm != null)
            {
                var link = avm.LinkToParent as IPneumaticColliderExtensionProvider;

                AttachActionToCollider(vm, link);
            }
        }

        private static void AttachActionToCollider(ColliderViewModel vm, IPneumaticColliderExtensionProvider link)
        {
            var s = Math.Sign(link.OnPos - link.OffPos);
            var max = Math.Abs(link.OnPos - link.OffPos);
            var direction = GetDirectionVector(link.Direction) * s;

            link.EvaluateCollision = (e) =>
            {
                double intersectValue = link.OnPos;
                bool state = link.Value;
                //var t = vm.CheckPanelIntersection(state, direction);
                var t = vm.CheckPanelIntersectionAsync(state, direction).Result;
                
                if (t.Item1 && (t.Item2 < max))
                {
                    intersectValue = t.Item2;
                    link.HasCollision = true;
                    link.CollisionOnPos = intersectValue * s;
                    vm.Collided = true;
                }
                else
                {
                    link.HasCollision = false;
                    vm.Collided = false;
                }
            };

            link.OnMovementCompleted = (e) =>
            {
                if (e.Value) vm.EvaluateOnState();
            };

            link.OnMovementStarting = (e) =>
            {
                if (!e.Value) vm.EvaluateOffState();
            };
        }

        private static Vector3D GetDirectionVector(LinkDirection d)
        {
            var direction = new Vector3D();

            switch (d)
            {
                case LinkDirection.X: direction.X = 1.0; break;
                case LinkDirection.Y: direction.Y = 1.0; break;
                case LinkDirection.Z: direction.Z = 1.0; break;
                default: break;
            }

            return direction;
        }

        //public static Rect3D? GetPanel()
        //{
        //    Transform3D panelTransform = null;
        //    Rect3D? panel = null;

        //    Messenger.Default.Send(new GetPanelPositionMessage()
        //    {
        //        SetData = (b, t, box) =>
        //        {
        //            if (b)
        //            {
        //                panelTransform = t;

        //                var p = panelTransform.Transform(box.Value.Location);
        //                panel = new Rect3D(p, box.Value.Size);
        //                //Messenger.Default.Send(new TraceBoxMessage() { Box = panel.Value, Brush = Brushes.Yellow });
        //            }
        //        }                    
        //    });

        //    return panel;
        //}

        private static HelixToolkit.Wpf.SharpDX.MeshGeometry3D GetMeshGeometry(double radius, List<Point3D> points)
        {
            var builder = new MeshBuilder();

            foreach (var p in points)
            {
                builder.AddSphere(p.ToVector3(), radius);
            }

            return builder.ToMesh();
        }


        private static Action GetGripperOnCollideAction(ColliderViewModel vm)
        {
            return null;
        }

        private static IMachineElementViewModel GetFirstAncestorWithPneumaticLink(IMachineElementViewModel vm)
        {
            if(vm != null)
            {
                if((vm.LinkToParent != null) && (vm.LinkToParent is TwoPositionLinkViewModel))
                {
                    return vm;
                }
                else
                {
                    return GetFirstAncestorWithPneumaticLink(vm.Parent);
                }
            }
            else
            {
                return null;
            }
        }

        private static IMachineElementViewModel GetFirstAncestorWithLinearPositionLink(IMachineElementViewModel vm)
        {
            if(vm != null)
            {
                if((vm.LinkToParent != null) && (vm.LinkToParent is LinearPositionViewModel))
                {
                    return vm;
                }
                else
                {
                    return GetFirstAncestorWithLinearPositionLink(vm.Parent);
                }
            }
            else
            {
                return null;
            }
        }

        private static IMachineElementViewModel GetFirstAncestorWithLinearPositionLink(IMachineElementViewModel vm, LinkDirection direction)
        {
            if (vm != null)
            {
                if ((vm.LinkToParent != null) && 
                    (vm.LinkToParent is LinearPositionViewModel lpvm) &&
                    (lpvm.Direction == direction))
                {
                    return vm;
                }
                else
                {
                    return GetFirstAncestorWithLinearPositionLink(vm.Parent, direction);
                }
            }
            else
            {
                return null;
            }
        }

    }
}
