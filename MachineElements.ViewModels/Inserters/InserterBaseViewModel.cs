using System.Windows.Media.Media3D;
using IStepObserver = MachineElements.ViewModels.Interfaces.Steps.IStepObserver;
using GalaSoft.MvvmLight.Ioc;
using StepExecutionDirection = MachineElements.ViewModels.Interfaces.Enums.StepExecutionDirection;

namespace MachineElements.ViewModels.Inserters
{
    public abstract class InserterBaseViewModel : MachineElementViewModel
    {
        IStepObserver _stepObserver;

        IStepObserver StepObserver => _stepObserver ?? (_stepObserver = SimpleIoc.Default.GetInstance<IStepObserver>());

        protected bool IsBackStepProgress => StepObserver.Direction == StepExecutionDirection.Back;

        protected bool IsFarwardStepProgress => StepObserver.Direction == StepExecutionDirection.Farward;


        protected Transform3D _chainTransform;

        public int InserterId { get; set; }

        public Point3D Position { get; set; }

        public Vector3D Direction { get; set; }

        public InserterBaseViewModel() : base()
        {

        }
    }
}
