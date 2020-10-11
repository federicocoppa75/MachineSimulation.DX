using System;
using CncViewer.Connection.Enums;
using CncViewer.Connection.Interfaces;
using CncViewer.Connection.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace CncViewer.Connection.ViewModels.Links
{
    public abstract class LinkViewModel : ViewModelBase
    {
        public int Id { get; private set; }
        public string Variable { get; private set; }
        abstract public LinkType Type { get; }

        public string Description { get; set; }

        public LinkViewModel(int id, string variable) : base()
        {
            Id = id;
            Variable = variable;

            MessengerInstance.Register<GetVariableToReadMessage>(this, OnGetVariableToReadMessage);
        }

        private void OnGetVariableToReadMessage(GetVariableToReadMessage msg) => msg.AddVariableAct(Id, Type, Variable);
    }

    public abstract class LinkViewModel<T> : LinkViewModel
    {
        private IVariableValueChangedObserver<T> _observer;

        private T _value;
        public T Value
        {
            get => _value;
            protected set => Set(ref _value, value, nameof(Value));
        }

        public LinkViewModel(int id, string variable) : base(id, variable)
        {
            _observer = SimpleIoc.Default.GetInstance<IVariableValueChangedObserver<T>>();

            MessengerInstance.Register<ValueChangedMessage<T>>(this, OnValueChangedMessage);
            MessengerInstance.Register<GetAllValuesMessage>(this, OnGetAllValuesMessage);
        }

        private void OnGetAllValuesMessage(GetAllValuesMessage msg) => _observer.ValueChanged(Id, Value);

        private void OnValueChangedMessage(ValueChangedMessage<T> msg)
        {
            if (msg.LinkId == Id)
            {
                Value = msg.Value;
                _observer.ValueChanged(Id, Value);
            }
        }
    }
}
