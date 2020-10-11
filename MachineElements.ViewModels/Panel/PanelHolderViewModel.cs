using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Messages.Generic;
using MachineElements.ViewModels.Messages.Panel;
using MachineElements.ViewModels.Messages.PanelHolder;
using MachineElements.ViewModels.Messages.Visibility;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using MachineElements.ViewModels.Colliders;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using MachineElements.ViewModels.Extensions;
using System.Linq;
using MachineElements.ViewModels.Helpers.UI;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Interfaces.Panel;
using SimpleIoc = GalaSoft.MvvmLight.Ioc.SimpleIoc;
using INotifierController = MachineElements.ViewModels.Interfaces.Collections.INotifierController;

namespace MachineElements.ViewModels.Panel
{
    public class PanelHolderViewModel : MachineElementViewModel
    {
        private static int _panelCount;

        private IPanelViewModel _panel;

        protected Func<Rect3D> _getBounds;

        private List<IPanelHooker> _hookers = new List<IPanelHooker>();

        private Transform3D _sourceTransformation;

        private IPanelHooker _activeHooker;

        private Matrix3D _residualTransform;

        private bool _residualTransformInitialized;

        public Point3D Position { get; set; }

        public PanelLoadType Corner { get; set; }

        public int PanelHolderId { get; set; }

        public INotifierController NotifierController { get; private set; }

        private IPanelViewModel _loadedPanel;

        public IPanelViewModel LoadedPanel
        {
            get => _loadedPanel; 
            set => Set(ref _loadedPanel, value, nameof(LoadedPanel)); 
        }

        private IPanelHooker _activePanelHooker;

        public IPanelHooker ActivePanelHooker
        {
            get => _activePanelHooker;
            set => Set(ref _activePanelHooker, value, nameof(ActivePanelHooker));
        }


        public PanelHolderViewModel() : base()
        {
            MessengerInstance.Register<GetAvailablePanelHoldersMessage>(this, OnGetAvailablePanelHoldersMessage);
            MessengerInstance.Register<LoadPanelMessage>(this, OnLoadPanelMessage);
            MessengerInstance.Register<UnloadPanelMessage>(this, OnUnloadPanelMessage);
            MessengerInstance.Register<PanelHolderVisibilityChangedMessage>(this, OnPanelHolderVisibilityChanged);
            MessengerInstance.Register<HookPanelMessage>(this, OnHookPanel);
            MessengerInstance.Register<UnhookPanelMessage>(this, OnUnhookPanel);
            MessengerInstance.Register<GetPanelPositionMessage>(this, OnGetPanelPosition);
            MessengerInstance.Register<GetPanelTransformMessage>(this, OnGetPanelTransformMessage);

            var oc = new Collections.ObservableCollection<IMachineElementViewModel>();
            NotifierController = oc;
            Children = oc;
        }

        private void OnGetAvailablePanelHoldersMessage(GetAvailablePanelHoldersMessage msg) => msg?.AvailableToolHolder(PanelHolderId, Name, Corner);

        protected virtual void ResetPanel()
        {
            _panel = null;
            _getBounds = null;
        }

        private void OnUnloadPanelMessage(UnloadPanelMessage msg)
        {
            if (msg.PanelHolderId == PanelHolderId)
            {
                ResetPanel();
                LoadedPanel = null;

                msg?.NotifyExecution?.Invoke(true);
                if (msg.BackNotifyId > 0) MessengerInstance.Send(new BackNotificationMessage() { DestinationId = msg.BackNotifyId });
            }
        }

        protected virtual void OnLoadPanelMessage(LoadPanelMessage msg)
        {
            if (msg.PanelHolderId == PanelHolderId)
            {
                var center = GetPanelCenter(msg.Length, msg.Width, msg.Height);

                _panel = SimpleIoc.Default.GetInstance<IPanelViewModel>();
                _panel.CenterX = center.X;
                _panel.CenterY = center.Y;
                _panel.CenterZ = center.Z;
                _panel.SizeX = msg.Length;
                _panel.SizeY = msg.Width;
                _panel.SizeZ = msg.Height;
                _panel.Visible = true;
                _panel.Parent = this;

                _panel.Initialize();

                _getBounds = () => _panel.Geometry.Bound.ToRect3D();
                _getBounds = () => _panel.Bound;

                LoadedPanel = _panel;

                msg?.NotifyExecution?.Invoke(true);
                if (msg.BackNotifyId > 0) MessengerInstance.Send(new BackNotificationMessage() { DestinationId = msg.BackNotifyId });
            }
        }

        protected Vector3D GetPanelCenter(double length, double width, double height)
        {
            Vector3D center;

            switch (Corner)
            {
                case PanelLoadType.Corner1:
                    center = new Vector3D(length / 2.0, width / 2.0, height / 2.0);
                    break;
                case PanelLoadType.Corner2:
                    center = new Vector3D(-length / 2.0, width / 2.0, height / 2.0);
                    break;
                case PanelLoadType.Corner3:
                    center = new Vector3D(-length / 2.0, -width / 2.0, height / 2.0);
                    break;
                case PanelLoadType.Corner4:
                    center = new Vector3D(length / 2.0, -width / 2.0, height / 2.0);
                    break;
                default:
                    center = new Vector3D();
                    break;
            }

            return center;
        }

        private void OnPanelHolderVisibilityChanged(PanelHolderVisibilityChangedMessage msg) => Visible = msg.Value;

        private void OnHookPanel(HookPanelMessage msg)
        {
            if (_panel != null)
            {
                if (_sourceTransformation == null) _sourceTransformation = this.GetChainTansform().CloneCurrentValue();


                if (_hookers.Count == 0)
                {
                    ChangeReference(_panel, _sourceTransformation, msg.Hooker.TotalTransformation);
                    _activeHooker = msg.Hooker;
                    _residualTransform = _panel.Transform.Value;
                    _residualTransform.Invert();
                    _residualTransformInitialized = true;
                    ActivePanelHooker = _activeHooker;

                }

                _hookers.Add(msg.Hooker);
            }
        }

 
        private void OnUnhookPanel(UnhookPanelMessage msg)
        {
            if (_panel != null)
            {
                _hookers.Remove(msg.Hooker);

                if (object.ReferenceEquals(_activeHooker, msg.Hooker))
                {
                    if (_hookers.Count > 0)
                    {
                        var h = _hookers.FirstOrDefault();

                        ChangeReference(_panel, msg.Hooker.TotalTransformation, h.TotalTransformation);
                        _activeHooker = h;
                        _residualTransform = _panel.Transform.Value;
                        _residualTransform.Invert();
                        _residualTransformInitialized = true;
                        ActivePanelHooker = _activeHooker;
                    }
                    else
                    {
                        ChangeReference(_panel, msg.Hooker.TotalTransformation, _sourceTransformation);

                        _sourceTransformation = null;
                        _activeHooker = null;
                        ActivePanelHooker = null;
                    }
                }
            }
        }

        //private void OnUnhookPanel(UnhookPanelMessage msg)
        //{
        //    if (_panel != null)
        //    {
        //        _hookers.Remove(msg.Hooker);

        //        if (object.ReferenceEquals(_activeHooker, msg.Hooker))
        //        {
        //            var toHooker = _hookers.Count > 0;
        //            var nextHooker = toHooker ? _hookers.FirstOrDefault() : null;
        //            var targetTransform = toHooker ? nextHooker.TotalTransformation : _sourceTransformation;
        //            var changeReferenceTransform = GetChangeReferenceTransform(_panel, msg.Hooker.TotalTransformation, targetTransform);

        //            msg.Hooker.UnhookPanel();

        //            if (_hookers.Count > 0)
        //            {
        //                //var h = _hookers.FirstOrDefault();

        //                //ChangeReference(_panel, msg.Hooker.TotalTransformation, h.TotalTransformation);
        //                //h.HookPanel(_panel);
        //                //_activeHooker = h;
        //                _panel.Transform = changeReferenceTransform;
        //                nextHooker.HookPanel(_panel);
        //                _residualTransform = _panel.Transform.Value;
        //                _residualTransform.Invert();
        //                _residualTransformInitialized = true;
        //            }
        //            else
        //            {
        //                //ChangeReference(_panel, msg.Hooker.TotalTransformation, _sourceTransformation);
        //                _panel.Transform = changeReferenceTransform;
        //                Children.Add(_panel);
        //                _sourceTransformation = null;
        //                _activeHooker = null;
        //            }
        //        }
        //    }
        //}

        private void ChangeReference(IMachineElementViewModel m, Transform3D source, Transform3D destination)
        {
            var tg = new Transform3DGroup();

            tg.Children.Add(m.Transform);
            tg.Children.Add(source);
            var td = destination.Value;
            td.Invert();
            tg.Children.Add(new MatrixTransform3D(td));

            m.Transform = new MatrixTransform3D(tg.Value);
        }

        //private Transform3D GetChangeReferenceTransform(IMachineElementViewModel m, Transform3D source, Transform3D destination)
        //{
        //    var tg = new Transform3DGroup();

        //    tg.Children.Add(m.Transform);
        //    tg.Children.Add(source);
        //    var td = destination.Value;
        //    td.Invert();
        //    tg.Children.Add(new MatrixTransform3D(td));

        //    return new MatrixTransform3D(tg.Value);
        //}

        private void OnGetPanelPosition(GetPanelPositionMessage msg)
        {
            if (_panel != null)
            {
                var box = _getBounds();
                var p = _panel.Transform.Transform(box.Location);

                if (_activeHooker != null)
                {
                    msg.SetData(true, _activeHooker.TotalTransformation, new Rect3D(p, box.Size));
                }
                else
                {
                    msg.SetData(true, this.GetChainTansform(), new Rect3D(p, box.Size));
                }

            }
            else
            {
                msg.SetData(false, null, null);
            }
        }

        private void OnGetPanelTransformMessage(GetPanelTransformMessage msg)
        {
            if (_panel != null)
            {
                if (_residualTransformInitialized)
                {
                    if (_activeHooker != null)
                    {
                        var ht = _activeHooker.TotalTransformation.Value;
                        var ir = _residualTransform;
                        ir.Invert();
                        var tg = ht * ir;

                        msg.SetData(true, tg);
                    }
                    else
                    {
                        msg.SetData(true, _residualTransform);
                    }
                }
                else
                {
                    DispatcherHelperEx.CheckBeginInvokeOnUI(() => msg.SetData(true, this.GetChainTansform().Value));
                }
            }
            else
            {
                msg.SetData(false, Matrix3D.Identity);
            }
        }
    }
}
