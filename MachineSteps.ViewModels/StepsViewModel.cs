using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MachineElements.ViewModels.Helpers.UI;
using MachineElements.ViewModels.Links.Movement;
using MachineSteps.Models.Actions;
using MachineSteps.ViewModels.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using IStepObserver = MachineElements.ViewModels.Interfaces.Steps.IStepObserver;
using MeVmMG = MachineElements.ViewModels.Messages.Generic;

namespace MachineSteps.ViewModels
{
    public class StepsViewModel : ViewModelBase
    {
        private IStepObserver _stepObserver;

        private bool _autoStepOver;
        private bool _multiChannel;
        ConcurrentDictionary<int, bool> _channelState = new ConcurrentDictionary<int, bool>();
        ConcurrentDictionary<int, int> _channelFreeBackNotifyId = new ConcurrentDictionary<int, int>();

        public ObservableCollection<StepViewModel> Steps { get; private set; } = new ObservableCollection<StepViewModel>();

        private StepViewModel _selected;

        public StepViewModel Selected
        {
            get { return _selected; }
            set
            {
                if((value != null) && (value.Channel > 0) && _autoStepOver && _multiChannel)
                {
                    var next = GetNextStep(0, (_selected != null) ? _selected.Index : 0);
                    var lastSelected = _selected;

                    if (Set(ref _selected, next, nameof(Selected)))
                    {
                        ManageSelectionChanged(_selected, lastSelected);
                    }
                }
                else
                {
                    var lastSelected = _selected;

                    if(Set(ref _selected, value, nameof(Selected)))
                    {
                        ManageSelectionChanged(_selected, lastSelected);
                    }
                }
            }
        }

        public StepsViewModel() : base()
        {
            _stepObserver = SimpleIoc.Default.GetInstance<IStepObserver>();

            MessengerInstance.Register<LoadStepsMessage>(this, OnLoadStepsMessage);
            MessengerInstance.Register<UnloadStepsMessage>(this, OnUnloadStepsMessage);
            MessengerInstance.Register<StepCompleteMessage>(this, OnStepCompleteMessage);
            MessengerInstance.Register<AutoStepOverChangedMessage>(this, OnAutoStepOverChangedMessage);
            MessengerInstance.Register<MaterialRemovalMessage>(this, OnMaterialRemovalMessage);
            MessengerInstance.Register<MultiChannelMessage>(this, OnMultiChannelMessage);
            MessengerInstance.Register<WaitForChannelFreeMessage>(this, OnWaitForChannelFreeMessage);
        }

        private void OnWaitForChannelFreeMessage(WaitForChannelFreeMessage msg)
        {
            if(_channelState.GetOrAdd(msg.Channel, false))
            {
                if (!_channelFreeBackNotifyId.TryAdd(msg.Channel, msg.BackNotifyId)) throw new InvalidOperationException();
            }
            else
            {
                MessengerInstance.Send(new MeVmMG.BackNotificationMessage() { DestinationId = msg.BackNotifyId });
            }
        }

        private void OnMultiChannelMessage(MultiChannelMessage msg)
        {
            _multiChannel = msg.Value;

            if (_multiChannel) LinearLinkMovementManager.ForceInitialize();
        }

        private void OnMaterialRemovalMessage(MaterialRemovalMessage msg) => LinearLinkMovementManager.EnableMaterialRemoval = msg.Active;

        private void OnAutoStepOverChangedMessage(AutoStepOverChangedMessage msg) => _autoStepOver = msg.Value;

        private void OnStepCompleteMessage(StepCompleteMessage msg)
        {
            if(_autoStepOver)
            {
                if(_multiChannel)
                {
                    OnStepCompleteMessageMultiChannel(msg);
                }
                else
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(50);

                    StepViewModel newSelection = GetNextStep();

                    if (newSelection != null) DispatcherHelperEx.CheckBeginInvokeOnUI(() => Selected = newSelection);
                    //if (newSelection != null) Selected = newSelection;
                });
                }
            }
        }

        private void OnStepCompleteMessageMultiChannel(StepCompleteMessage msg)
        {
            Task.Run(async () =>
            {
                await Task.Delay(50);

                if (msg.Channel > 0)
                {
                    var step = GetNextStep(msg.Channel, msg.Index);

                    if (_channelState.AddOrUpdate(msg.Channel, step != null, (k, v) => step != null))
                    {
                        step.UpdateLazys();
                        step.ExecuteFarward();
                    }
                    else
                    {
                        if (_channelFreeBackNotifyId.TryRemove(msg.Channel, out int id))
                        {
                            MessengerInstance.Send(new MeVmMG.BackNotificationMessage() { DestinationId = id });
                        }
                    }
                }
                else
                {
                    StepViewModel newSelection = GetNextStep();

                    if (newSelection != null) DispatcherHelperEx.CheckBeginInvokeOnUI(() => Selected = newSelection);
                }
            });
        }

        private StepViewModel GetNextStep()
        {
            StepViewModel newSelection = null;

            if (_selected == null)
            {
                newSelection = Steps[0];
            }
            else
            {
                int index = Steps.IndexOf(Selected) + 1;

                if (index < Steps.Count())
                {
                    newSelection = Steps[index];
                }
            }

            return newSelection;
        }

        private StepViewModel GetNextStep(int channel, int fromIndex)
        {
            StepViewModel nextStep = null;

            for (int i = fromIndex + 1; i < Steps.Count; i++)
            {
                if(Steps[i].Channel == channel)
                {
                    nextStep = Steps[i];
                    break;
                }
                else if(IsChannelWaiter(Steps[i], channel))
                {
                    break;
                }
            }

            return nextStep;
        }

        private bool IsChannelWaiter(StepViewModel step, int channel)
        {
            bool result = false;

            if((step.FarwardActions.Count == 1) && 
                (step.FarwardActions[0].Action is ChannelWaiterAction action) && 
                (action.ChannelToWait == channel))
            {
                result = true;
            }

            return result;
        }

        private void OnUnloadStepsMessage(UnloadStepsMessage msg)
        {
            if(Steps.Count > 0)
            {
                Selected = Steps[0];
                Steps.Clear();
                Selected = null;
            }
        }

        private void OnLoadStepsMessage(LoadStepsMessage msg)
        {
            if((msg != null) && (msg.Steps != null) && (msg.Steps.Count > 0))
            {
                Steps.Add(new StepViewModel(-1, "Start", "Condizione iniziale"));

                for (int i = 0; i < msg.Steps.Count; i++)
                {
                    Steps.Add(new StepViewModel(msg.Steps[i], i + 1));
                }

                UpdateEvolutionTime();
            }
        }

        private void ManageSelectionChanged(StepViewModel selected, StepViewModel lastSelected)
        {
            if(lastSelected == null)
            {
                ManageFarwardSelectionChanged(selected, Steps[0]);
            }
            else if(selected == null)
            {
                // per il momento non faccio nulla
            }
            else if(selected.Index > lastSelected.Index)
            {
                ManageFarwardSelectionChanged(selected, lastSelected);
            }
            else if (selected.Index < lastSelected.Index)
            {
                ManageBackSelectionChanged(selected, lastSelected);
            }

            MessengerInstance.Send(new UpdateStepTimeMessage() { Time = (Selected != null) ? Selected.EvolutionTime : 0.0 });
        }

        private void ManageBackSelectionChanged(StepViewModel selected, StepViewModel lastSelected)
        {
            MessengerInstance.Send(new SuspendPlaybackSettingsMessage());

            for (int i = lastSelected.Index; i > selected.Index; i--)
            {
                _stepObserver.SetBackIndex(i);
                Steps[i].ExecuteBack();
            }

            MessengerInstance.Send(new ResumePlaybackSettingsMessage());

            if(Selected.Index == 0)
            {
                _channelFreeBackNotifyId.Clear();
                _channelState.Clear();
            }
        }

        private void ManageFarwardSelectionChanged(StepViewModel selected, StepViewModel lastSelected)
        {
            if(_autoStepOver && _multiChannel)
            {
                ManageFarwardSelectionChangedDynamic(selected, lastSelected);
            }
            else
            {
                ManageFarwardSelectionChangedStatic(selected, lastSelected);
            }
        }

        private void ManageFarwardSelectionChangedStatic(StepViewModel selected, StepViewModel lastSelected)
        {
            for (int i = lastSelected.Index + 1; i <= selected.Index; i++)
            {
                var svm = Steps[i];

                _stepObserver.SetFarwardIndex(i);
                svm.UpdateLazys();
                svm.ExecuteFarward();
            }
        }

        private void ManageFarwardSelectionChangedDynamic(StepViewModel selected, StepViewModel lastSelected)
        {
            HashSet<int> set = new HashSet<int>();

            for (int i = lastSelected.Index + 1; i <= selected.Index; i++)
            {
                var svm = Steps[i];

                if(svm.Channel == 0)
                {
                    _stepObserver.SetFarwardIndex(i);
                    svm.UpdateLazys();
                    svm.ExecuteFarward();

                    break;
                }
                else if((svm.Channel > 0) && !set.Contains(svm.Channel))
                {
                    set.Add(svm.Channel);

                    _channelState.AddOrUpdate(svm.Channel, true, (k, v) => true);

                    Task.Run(() =>
                    {
                        svm.UpdateLazys();
                        svm.ExecuteFarward();
                    });
                }
            }
        }

        private void UpdateEvolutionTime()
        {
            double time = 0.0;

            for (int i = 0; i < Steps.Count; i++)
            {
                time += Steps[i].Duration;
                Steps[i].EvolutionTime = time;
            }
        }

        private bool IsSelectedFirst() => (Selected != null) ? Steps.IndexOf(Selected) == 0 : false;
    }
}
