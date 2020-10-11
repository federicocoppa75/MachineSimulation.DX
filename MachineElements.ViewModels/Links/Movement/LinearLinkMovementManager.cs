using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf.SharpDX.Controls;
using MachineElements.ViewModels.Helpers.UI;
using MachineElements.ViewModels.Messages.Links;
using MachineElements.ViewModels.Messages.Panel;
using MachineElements.ViewModels.Messages.ToolState;
using MachineViewer.Plugins.Common.Models.Links.Interpolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Tools.Models.Enums;
using IMaterialRemovalMessageSender = MachineElements.ViewModels.Interfaces.Panel.IMaterialRemovalMessageSender;
using DiskOnConeTool = Tools.Models.DiskOnConeTool;
using DiskTool = Tools.Models.DiskTool;
using IStepObserver = MachineElements.ViewModels.Interfaces.Steps.IStepObserver;

namespace MachineElements.ViewModels.Links.Movement
{
    public static class LinearLinkMovementManager
    {
        public enum ArcComponentItem
        {
            X,
            Y
        };

        static List<LinearLinkMovementItem> _items = new List<LinearLinkMovementItem>();

        static Dictionary<int, LinksMovementsGroup> _itemsGroups = new Dictionary<int, LinksMovementsGroup>();

        static object _lockObj1 = new object();

        static object _lockObj2 = new object();

        static CompositionTargetEx _compositeHelper = new CompositionTargetEx();

        static int _pendingMovement = 0;

        static DateTime _lastMaterialRemovalProcess = DateTime.Now;

        static bool _removalTaskActive;

        static ManualResetEventSlim _removalTaskManualResetEvent = new ManualResetEventSlim();

        static IMaterialRemovalMessageSender _materialRemovalMessageSender = SimpleIoc.Default.GetInstance<IMaterialRemovalMessageSender>();

        public static bool EnableMaterialRemoval { get; set; }

        static LinearLinkMovementManager()
        {
            _compositeHelper.Rendering += OnRendering;
        }

        public static void Add(int linkId, double value, double targetValue, double duration)
        {
            lock (_lockObj1)
            {
                _items.Add(new LinearLinkMovementItem(linkId, value, targetValue, duration));
            }
        }

        public static void Add(int groupId, int linkId, double value, double targetValue, double duration)
        {
            lock(_lockObj2)
            {
                if (!_itemsGroups.TryGetValue(groupId, out LinksMovementsGroup group))
                {
                    group = new LinksMovementsGroup(groupId, duration);
                    _itemsGroups.Add(groupId, group);
                }

                group.Add(linkId, value, targetValue);
            }
        }

        public static void Add(int linkId, double targetValue, double duration, ArcComponentData data)
        {
            lock (_lockObj2)
            {
                if (!_itemsGroups.TryGetValue(data.GroupId, out LinksMovementsGroup group))
                {
                    group = new LinksMovementsGroup(data.GroupId, duration);
                    _itemsGroups.Add(data.GroupId, group);
                }

                group.Add(linkId, targetValue, data);
            }
        }

        public static void ForceMaterialRemoval()
        {
            if (EnableMaterialRemoval) Interlocked.Exchange(ref _pendingMovement, 1);
        }

        private static void OnRendering(object sender, RenderingEventArgs e) => Evaluate();

        private static void Evaluate()
        {
            var now = DateTime.Now;
            var elapse = now - _lastMaterialRemovalProcess;

            if (elapse > TimeSpan.FromMilliseconds(100))
            {
                EvaluateItems(now);
                EvaluateGroups(now);
                EvaluateMaterialRemovalEvo(now);
            }
        }

        private static void EvaluateGroups(DateTime now)
        {
            lock(_lockObj2)
            {
                if (_itemsGroups.Count > 0) Interlocked.Exchange(ref _pendingMovement, 1);

                foreach (var ig in _itemsGroups.Values)
                {
                    ig.Progress(now);

                    DispatcherHelperEx.CheckBeginInvokeOnUI(() =>
                    {
                        ig.Items.ForEach((i) => Messenger.Default.Send(new UpdateLinearLinkStateToTargetMessage(i.LinkId, i.ActualValue, ig.IsCompleted)));
                    });
                }

                _itemsGroups = _itemsGroups.Where(ig => !ig.Value.IsCompleted).ToDictionary(kp => kp.Key, kp => kp.Value);
            }
        }

        private static void EvaluateItems(DateTime now)
        {
            lock (_lockObj1)
            {
                if (_items.Count > 0) Interlocked.Exchange(ref _pendingMovement, 1);

                _items.ForEach(i =>
                {
                    i.Progress(now);

                    DispatcherHelperEx.CheckBeginInvokeOnUI(() =>
                    {
                        Messenger.Default.Send(new UpdateLinearLinkStateToTargetMessage(i.LinkId, i.ActualValue, i.IsCompleted));
                    });
                });

                _items = _items.Where((ii) => !ii.IsCompleted).ToList();
            }
        }

        private static void EvaluateMaterialRemoval(DateTime now)
        {
            if (EnableMaterialRemoval)
            {
                var elapse = now - _lastMaterialRemovalProcess;

               
                if (Interlocked.CompareExchange(ref _pendingMovement, 0, 1) == 1)
                {
                    Messenger.Default.Send(new GetPanelTransformMessage()
                    {
                        SetData = (b, t) =>
                        {
                            if (b && (t != null))
                            {
                                //EvaluateMaterialRemovalForRoutTool(t);
                                EvaluateMaterialRemovalForTool(t);
                            }
                        }
                    });
                }

                //Task.Run(() => Messenger.Default.Send(new ProcessPendingRemovalMessage()));
                _materialRemovalMessageSender.SendProcessPendingRemovalMessage();
                _lastMaterialRemovalProcess = now;
                 
            }
        }

        private static void EvaluateMaterialRemovalEvo(DateTime now)
        {
            if(EnableMaterialRemoval)
            {
                if(!_removalTaskActive)
                {
                    Task.Run(() =>
                    {
                        _removalTaskActive = true;

                        while (true)
                        {
                            if (Interlocked.CompareExchange(ref _pendingMovement, 0, 1) == 1)
                            {
                                DispatcherHelperEx.CheckBeginInvokeOnUI(() =>
                                {
                                    Messenger.Default.Send(new GetPanelTransformMessage()
                                    {
                                        SetData = (b, t) =>
                                        {
                                            //if (b) EvaluateMaterialRemovalForRoutTool(t);
                                            if (b) EvaluateMaterialRemovalForTool(t);
                                        }
                                    });
                                });
                            }
                            else
                            {
                                _removalTaskManualResetEvent.Wait();
                                _removalTaskManualResetEvent.Reset();
                            }

                            if (!EnableMaterialRemoval) break;
                        }

                        _removalTaskActive = false;
                    });
                }
                else
                {
                    _removalTaskManualResetEvent.Set();
                }

                //Messenger.Default.Send(new ProcessPendingRemovalMessage());
                _materialRemovalMessageSender.SendProcessPendingRemovalMessage();
            }
            else
            {
                if (!_removalTaskManualResetEvent.IsSet) _removalTaskManualResetEvent.Set();
            }
        }

        private static void EvaluateMaterialRemovalForRoutTool(Matrix3D panelTransform)
        {
            var invT = panelTransform;

            invT.Invert();

            Messenger.Default.Send(new GetActiveRoutToolMessage()
            {
                SetData = (p, d, t, id) =>
                {
                    Task.Run(() =>
                    {
                        var pos = invT.Transform(p);

                        if (t.ToolType == ToolType.DiskOnCone)
                        {
                            var tt = t as DiskOnConeTool;
                            var s = tt.PostponemntLength - (tt.CuttingThickness - tt.BodyThickness) / 2.0;

                            _materialRemovalMessageSender.SendRoutToolMoveMessage(id,
                                                                                 pos + d * s,
                                                                                 d,
                                                                                 tt.CuttingThickness,
                                                                                 tt.Diameter / 2.0);
                        }
                        else if(t.ToolType == ToolType.Disk)
                        {
                            var tt = t as DiskTool;
                            var s = (tt.CuttingThickness - tt.BodyThickness) / 2.0;

                            _materialRemovalMessageSender.SendRoutToolMoveMessage(id,
                                                                                 pos - d * s,
                                                                                 d,
                                                                                 tt.CuttingThickness,
                                                                                 tt.Diameter / 2.0);
                        }
                        else
                        {
                            _materialRemovalMessageSender.SendRoutToolMoveMessage(id,
                                                                                 pos,
                                                                                 d,
                                                                                 t.GetTotalLength(),
                                                                                 t.GetTotalDiameter() / 2.0);
                        }
                    });
                }

            });
        }

        private static void EvaluateMaterialRemovalForTool(Matrix3D panelTransform)
        {
            var invT = panelTransform;

            invT.Invert();

            Messenger.Default.Send(new GetActiveRoutToolMessage()
            {
                SetData = (p, d, t, id) =>
                {
                    Task.Run(() =>
                    {
                        var pos = invT.Transform(p);

                        if (t.ToolType == ToolType.DiskOnCone)
                        {
                            var tt = t as DiskOnConeTool;
                            var s = tt.PostponemntLength - (tt.CuttingThickness - tt.BodyThickness) / 2.0;

                            _materialRemovalMessageSender.SendToolMoveMessage(id,
                                                                              pos + d * s,
                                                                              d,
                                                                              tt.CuttingThickness,
                                                                              tt.Diameter / 2.0);

                        }
                        else if(t.ToolType == ToolType.Disk)
                        {
                            var tt = t as DiskTool;
                            var s = (tt.CuttingThickness - tt.BodyThickness) / 2.0;

                            _materialRemovalMessageSender.SendToolMoveMessage(id,
                                                                              pos - d * s,
                                                                              d,
                                                                              tt.CuttingThickness,
                                                                              tt.Diameter / 2.0);
                        }
                        else
                        {
                            _materialRemovalMessageSender.SendToolMoveMessage(id,
                                                                              pos,
                                                                              d,
                                                                              t.GetTotalLength(),
                                                                              t.GetTotalDiameter() / 2.0);
                        }

                    });
                }

            });
        }


    }
}
