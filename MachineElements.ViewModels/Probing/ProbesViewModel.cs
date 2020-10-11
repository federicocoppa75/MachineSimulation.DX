using GalaSoft.MvvmLight;
using MachineElements.ViewModels.Messages.Probe;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MachineElements.ViewModels.Probing
{
    public class ProbesViewModel : ViewModelBase
    {
        public ObservableCollection<ProbeViewModel> Probes { get; set; } = new ObservableCollection<ProbeViewModel>();

        public ProbesViewModel() : base()
        {
            MessengerInstance.Register<AddProbeMessage>(this, OnAddProbeMessage);
            MessengerInstance.Register<AddPointDistanceProbeMessage>(this, OnAddPointDistanceProbeMessage);
            MessengerInstance.Register<RemoveSelectedProbeMessage>(this, OnRemoveSelectedProbeMessage);
            MessengerInstance.Register<CanExecuteAddPointDistanceMessage>(this, OnCanExecuteAddPointDistanceMessage);
            MessengerInstance.Register<CanExecuteRemoveProbeMessage>(this, OnCanExecuteRemoveProbeMessage);
        }

        private void OnCanExecuteRemoveProbeMessage(CanExecuteRemoveProbeMessage msg) => msg.SetValue(GetSelected().Any());

        private void OnCanExecuteAddPointDistanceMessage(CanExecuteAddPointDistanceMessage msg)
        {
            var result = false;
            var pbrs = GetSelected();

            if (pbrs.Count() == 2)
            {
                result = pbrs.All((p) => p is PointProbeViewModel);
            }

            msg.SetValue(result);
        }

        private void OnRemoveSelectedProbeMessage(RemoveSelectedProbeMessage obj)
        {
            var prbs = GetSelected().ToList();

            foreach (var item in prbs)
            {
                item.DetachFromParent();
                Probes.Remove(item);
            }
        }

        private void OnAddPointDistanceProbeMessage(AddPointDistanceProbeMessage msg)
        {
            var pts = GetSelected().Select((p) => p as PointProbeViewModel).ToArray();
            var vm = PointsDistanceViewModel.Create(pts[0], pts[1]);

            pts[0].Children.Add(vm);

            Probes.Add(vm);
        }

        private void OnAddProbeMessage(AddProbeMessage msg) => Probes.Add(msg.Probe);

        private IEnumerable<ProbeViewModel> GetSelected() => Probes.Where((p) => p.IsSelected);
    }
}
