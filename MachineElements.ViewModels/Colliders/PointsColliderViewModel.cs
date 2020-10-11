using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Extensions;
using MachineElements.ViewModels.Messages.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Ray3D = global::SharpDX.Ray;
using Box3 = global::SharpDX.BoundingBox;
using Vector3 = global::SharpDX.Vector3;
using Sphere3 = global::SharpDX.BoundingSphere;
using ContainmentType = global::SharpDX.ContainmentType;
using IPanelViewModel = MachineElements.ViewModels.Interfaces.Panel.IPanelViewModel;

namespace MachineElements.ViewModels.Colliders
{
    public class PointsColliderViewModel : ColliderViewModel, IPanelHooker
    {
        private bool _panelClamped;

        public double Radius { get; set; }

        public List<Point3D> Points { get; set; }


        public PointsColliderViewModel() : base()
        {
        }

        public override Tuple<bool, double> CheckPanelIntersection(bool targetLinkState, Vector3D linkDirection)
        {
            bool result = false;
            double intersectValue = 0.0;
            var zDir = new Vector3D(0.0, 0.0, 1.0);

            if (targetLinkState)
            {
                var ptPanel = ColliderExtensions.GetPanel();

                if (ptPanel.HasValue)
                {
                    var retPanel = ptPanel.Value;
                    var panelMin = retPanel.Location.ToVector3();
                    var panelMax = panelMin + new Vector3((float)retPanel.SizeX, (float)retPanel.SizeY, (float)retPanel.SizeZ);
                    var panel = new Box3(panelMin, panelMax);
                    var tt = TotalTransformation.Value;
                    var planePoint = panelMax;

                    foreach (var p in Points)
                    {
                        var pp = tt.Transform(p);
                        var ray = new Ray3D(pp.ToVector3(), linkDirection.ToVector3());

                        if (ray.PlaneIntersection(planePoint, zDir.ToVector3(), out Vector3 intersect) &&
                            panel.Intersects(new Sphere3(intersect, 0.001f)))
                        {
                            var pi = intersect.ToPoint3D();
                            result = true;
                            intersectValue = Vector3D.DotProduct(pi - pp, linkDirection);
                            break;

                        }
                    }
                }
            }

            return new Tuple<bool, double>(result, intersectValue);
        }

        public override Task<Tuple<bool, double>> CheckPanelIntersectionAsync(bool targetLinkState, Vector3D linkDirection)
        {
            if (targetLinkState)
            {
                var ptPanel = ColliderExtensions.GetPanel();

                if (ptPanel.HasValue)
                {
                    if(Points.Count > 0)
                    {
                        var zDir = GetPanelFaceDirectionForLinkImpact(linkDirection);
                        var retPanel = ptPanel.Value;
                        var panelMin = retPanel.Location.ToVector3();
                        var panelMax = panelMin + new Vector3((float)retPanel.SizeX, (float)retPanel.SizeY, (float)retPanel.SizeZ);
                        var panel = new Box3(panelMin, panelMax);
                        var tt = TotalTransformation.Value;
                        var planePoint = panelMax;
                        var tasks = new List<Task<Tuple<bool, double>>>();

                        foreach (var p in Points)
                        {
                            var point = p;

                            tasks.Add(Task.Run(() =>
                            {
                                var pp = tt.Transform(point);
                                var ray = new Ray3D(pp.ToVector3(), linkDirection.ToVector3());

                                if (ray.PlaneIntersection(planePoint, zDir.ToVector3(), out Vector3 intersect) &&
                                    panel.Intersects(new Sphere3(intersect, 0.001f)))
                                {
                                    var pi = intersect.ToPoint3D();
                                    var intersectValue = Vector3D.DotProduct(pi - pp, linkDirection);
                                    return new Tuple<bool, double>(true, intersectValue);
                                }
                                else
                                {
                                    return new Tuple<bool, double>(false, 0.0);
                                }
                            }));
                        }

                        return Task.WhenAll(tasks).ContinueWith((t) => t.Result.FirstOrDefault(r => r.Item1) ?? new Tuple<bool, double>(false, 0.0));
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("The number of points of collider must be greater than 0!");
                    }
                }
            }
            
            return Task.FromResult(new Tuple<bool, double>(false, 0.0));
        }

        private Vector3D GetPanelFaceDirectionForLinkImpact(Vector3D linkDirection)
        {
            if ((linkDirection.X == 0.0) && (linkDirection.Y == 0.0) && (linkDirection.Z < 0.0))
            {
                return new Vector3D(0.0, 0.0, 1.0);
            }
            else if ((linkDirection.X == 0.0) && (linkDirection.Y < 0.0) && (linkDirection.Z == 0.0))
            {
                return new Vector3D(0.0, 1.0, 0.0);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private double GetPanelSizeForLinkImpact(Rect3D panel, Vector3D linkDirection)
        {
            if ((linkDirection.X == 0.0) && (linkDirection.Y == 0.0) && (linkDirection.Z < 0.0))
            {
                return panel.SizeZ;
            }
            else if ((linkDirection.X == 0.0) && (linkDirection.Y < 0.0) && (linkDirection.Z == 0.0))
            {
                return panel.SizeY;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override void EvaluateOnState()
        {
            if((Type == ColliderType.Gripper) && Collided)
            {
                _panelClamped = true;
                Messenger.Default.Send(new HookPanelMessage() { Hooker = this });
            }
        }

        public override void EvaluateOffState()
        {
            if((Type == ColliderType.Gripper) && _panelClamped)
            {
                _panelClamped = false;
                Messenger.Default.Send(new UnhookPanelMessage() { Hooker = this });
            }
        }

        public void HookPanel(IPanelViewModel panel)
        {
            //NotifierController.EnableNotify = false;
            Children.Add(panel);
            //NotifierController.EnableNotify = true;
        }

        public IPanelViewModel UnhookPanel()
        {
            var panel = Children[0];

            //Children.Clear();
            //NotifierController.EnableNotify = false;
            Children.Remove(panel);
            //NotifierController.EnableNotify = true;

            return panel as IPanelViewModel;
        }
    }
}
