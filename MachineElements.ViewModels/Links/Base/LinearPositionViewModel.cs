using System;
using MachineElements.ViewModels.Interfaces.Enums;
using MachineElements.ViewModels.Interfaces.Links;
using MachineElements.ViewModels.Messages.Generic;
using MachineElements.ViewModels.Messages.Links.Gantry;

namespace MachineElements.ViewModels.Links.Base
{
    public class LinearPositionViewModel : DynamicLinkViewModel, ILinearLinkGantryMaster, IUpdatableValueLink<double>
    {
        class GantryData
        {
            public IUpdatableValueLink<double> Slave { get; set; }
            public double Offset { get; set; }

            public void SetSlaveValue(double value)
            {
                if (Slave != null) Slave.Value = value + Offset;
            }
        }

        private GantryData _gantryData;

        private int _backNotifyId = 0;

        public double Min { get; set; }
        public double Max { get; set; }
        public double Pos { get; set; }

        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                double oldValue = _value;

                if (Set(ref _value, value, nameof(Value)))
                {
                    //OldValue = oldValue;
                    OnValueChanged();
                    ValueChanged?.Invoke(this, _value);
                }
            }
        }

        public override LinkType LinkType => LinkType.LinearPosition;

        //[Browsable(false)]
        //public double OldValue { get; private set; }

        //[Browsable(false)]
        public EventHandler<double> ValueChanged;

        protected LinearPositionViewModel() : base()
        {
            MessengerInstance.Register<LinearPositionGantryOffMessage>(this, OnGantryOffMessage);
            MessengerInstance.Register<LinearPositionGantryOnMessage>(this, OnGantryOnMessage);
        }

        protected virtual void OnValueChanged() 
        {
            if (_gantryData != null) _gantryData.SetSlaveValue(Value);
        }

        public static LinearPositionViewModel Create() => new LinearPositionViewModel();

        private void OnGantryOffMessage(LinearPositionGantryOffMessage msg)
        {
            if (msg.MasterId == Id)
            {
                ResetGantry();
                if (msg.BackNotifyId > 0) MessengerInstance.Send(new BackNotificationMessage() { DestinationId = msg.BackNotifyId });
            }
        }

        private void OnGantryOnMessage(LinearPositionGantryOnMessage msg)
        {
            if ((msg.MasterId == Id) || (msg.SlaveId == Id))
            {
                bool execute = false;

                if (msg.MasterId == Id)
                {
                    msg.Master = this;
                    execute = true;
                }
                else if (msg.SlaveId == Id && !msg.UnhookedSlave)
                {
                    msg.Slave = this;
                    execute = true;
                }

                if (execute && msg.IsReady)
                {
                    msg.Execute();
                    if (msg.BackNotifyId > 0) MessengerInstance.Send(new BackNotificationMessage() { DestinationId = msg.BackNotifyId });
                }
            }
        }

        public void ResetGantry() => _gantryData = null;

        public void SetGantrySlave(IUpdatableValueLink<double> slave)
        {
            double offset = (slave != null) ? slave.Value - Value : 0.0;

            _gantryData = new GantryData()
            {
                Slave = slave,
                Offset = offset
            };
        }
    }
}
