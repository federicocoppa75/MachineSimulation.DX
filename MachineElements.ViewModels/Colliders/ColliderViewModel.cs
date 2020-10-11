using MachineElements.ViewModels.Enums;
using MachineElements.ViewModels.Extensions;
using MachineElements.ViewModels.Interfaces;
using MachineElements.ViewModels.Messages.Visibility;
using System;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using INotifierController = MachineElements.ViewModels.Interfaces.Collections.INotifierController;


namespace MachineElements.ViewModels
{
    public abstract class ColliderViewModel : MachineElementViewModel, IMachineElementViewModel
    {
        private Transform3D _chainTransform;

        public ColliderType Type { get; set; }

        public Transform3D TotalTransformation => _chainTransform ?? (_chainTransform = this.GetChainTansform());

        public bool Collided { get; set; }

        public INotifierController NotifierController { get; private set; }

        public abstract Tuple<bool, double> CheckPanelIntersection(bool targetLinkState, Vector3D linkDirection);

        public abstract Task<Tuple<bool, double>> CheckPanelIntersectionAsync(bool targetLinkState, Vector3D linkDirection);
        
        public abstract void EvaluateOnState();

        public abstract void EvaluateOffState();

        public ColliderViewModel() : base()
        {
            MessengerInstance.Register<CollidersVisibilityChangedMessage>(this, OnColliderVisibilityChanged);

            // normalmente non è necessaria la visualizzazione
            Visible = false;

            var oc = new Collections.ObservableCollection<IMachineElementViewModel>();
            NotifierController = oc;
            Children = oc;
        }

        private void OnColliderVisibilityChanged(CollidersVisibilityChangedMessage msg) => Visible = msg.Value;
    }
}
