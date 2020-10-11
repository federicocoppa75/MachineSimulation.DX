using GalaSoft.MvvmLight.Messaging;
using MachineElements.ViewModels.Interfaces.Enums;
using MachineElements.ViewModels.Interfaces.Messages.Steps;
using MachineElements.ViewModels.Interfaces.Steps;

namespace MaterialRemoval.ViewModels.Steps
{
    public class StepObserver : IStepObserver
    {
        private object _lockObj = new object();

        private int _index;
        public int Index
        {
            get
            {
                lock(_lockObj)
                {
                    return _index;
                }
            }
            private set
            {
                lock(_lockObj)
                {
                    _index = value;
                }
            }
        }
        public StepExecutionDirection Direction { get; private set; }

        public StepObserver()
        {

        }

        public void SetFarwardIndex(int index)
        {
            Index = index;
            Direction = StepExecutionDirection.Farward;
        }

        public void SetBackIndex(int index)
        {
            Index = index;
            Direction = StepExecutionDirection.Back;

            Messenger.Default.Send(new BackStepMessage() { Index = index });
        }
    }
}
