using GalaSoft.MvvmLight.Ioc;
using MachineElements.ViewModels.Interfaces.Messages.Steps;
using System;
using System.Collections.Generic;
using System.Text;
using IStepObserver = MachineElements.ViewModels.Interfaces.Steps.IStepObserver;

namespace MachineElements.ViewModels.Inserters
{
    public class InserterObjectViewModel : MachineElementViewModel
    {
        private IStepObserver _stepObserver;

        private int _stepIndex;

        public InserterObjectViewModel()
        {
            _stepObserver = SimpleIoc.Default.GetInstance<IStepObserver>();
            _stepIndex = _stepObserver.Index;

            MessengerInstance.Register<BackStepMessage>(this, OnBackStepMessage);
        }

        private void OnBackStepMessage(BackStepMessage msg)
        {
            if((_stepIndex == msg.Index) && (Parent != null))
            {
                Parent.Children.Remove(this);
            }
        }
    }
}
