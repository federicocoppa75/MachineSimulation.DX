using GalaSoft.MvvmLight.Messaging;
using MachineElements.ViewModels.Messages.Generic;
using MachineSteps.Models.Actions;
using MachineSteps.ViewModels.Extensions;
using MachineSteps.ViewModels.Messages;
using MachineSteps.ViewModels.Models;

namespace MachineSteps.ViewModels
{
    public class ActionViewModel
    {
        static int _idSeed = 1;

        public int Id { get; private set; }

        public BaseAction Action { get; private set; }

        private bool _durationIsValid;
        private double _duration;

        public double Duration
        {
            get
            {
                if (!_durationIsValid) InitDuration();
                return _duration;
            }
        }

        public bool IsCompleted { get; set; }

        public ActionViewModel(BaseAction action)
        {
            Id = _idSeed++;
            Action = action;

            Messenger.Default.Register<BackNotificationMessage>(this, OnBackNotificationMessage);
        }

        public void Execute(bool notifyExecution = false) => Action.ExecuteAction(notifyExecution ? Id : 0);

        public void UpdateLazy()
        {
            if(Action is ILazyAction la)
            {
                if (!la.IsUpdated) la.Update();
            }
        }

        private void InitDuration()
        {
            _duration = Action.GetDuration();
            _durationIsValid = true;
        }

        private void OnBackNotificationMessage(BackNotificationMessage msg)
        {
            if(Id == msg.DestinationId)
            {
                IsCompleted = true;
                Messenger.Default.Send(new ActionCompleteMessage());
            }
        }
    }
}
