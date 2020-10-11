﻿using MachineElements.ViewModels.Interfaces.Links;
using MachineElements.ViewModels.Links.Movement;
using MachineElements.ViewModels.Messages.Generic;
using MachineElements.ViewModels.Messages.Links;
using MachineElements.ViewModels.Messages.Links.Gantry;
using System.ComponentModel;

namespace MachineElements.ViewModels.Links.Evo
{
    public class LinearPositionViewModel : MachineElements.ViewModels.Links.Base.LinearPositionViewModel, ILinkViewModelDescriptionProvider, IUpdatableValueLink<double>, ILinearLinkGantryMaster
    {
        class GantryData
        {
            public IUpdatableValueLink<double> Slave{ get; set; }
            public double Offset { get; set; }

            public void SetSlaveValue(double value)
            {
                if(Slave != null) Slave.Value = value + Offset;
            }
        }

        private GantryData _gantryData;

        private int _backNotifyId = 0;

        public string Description { get; set; }

        [Browsable(false)]
        public bool IsPneumaticTransactionEnabled { get; set; }

        public LinearPositionViewModel() : base()
        {
            MessengerInstance.Register<UpdateLinearLinkStateMessage>(this, OnUpdateStateMessage);
            MessengerInstance.Register<ReadLinearLinkStateMessage>(this, OnReadStateMessage);
            MessengerInstance.Register<LinearPositionGantryOffMessage>(this, OnGantryOffMessage);
            MessengerInstance.Register<LinearPositionGantryOnMessage>(this, OnGantryOnMessage);
            MessengerInstance.Register<EnablePneumaticTransactionMessage>(this, OnEnablePneumaticTransactionMessage);
            MessengerInstance.Register<MoveLinearLinkMessage>(this, OnMoveLinearLinkMessage);
            MessengerInstance.Register<LinearInterpolationLinkMessage>(this, OnLinearInterpolationLinkMessage);
            MessengerInstance.Register<ArcInterpolationLinkMessage>(this, OnArcInterpolationLinkMessage);
            MessengerInstance.Register<UpdateLinearLinkStateToTargetMessage>(this, OnUpdateLinearLinkStateToTargetMessage);
            MessengerInstance.Register<CheckLinkPresenceMessage>(this, OnCheckLinkPresenceMessage);
            MessengerInstance.Register<ReadLinkLimitsMessage>(this, OnReadLinkLimitsMessage);
        }

        public static LinearPositionViewModel Create() => new LinearPositionViewModel();

        private void OnEnablePneumaticTransactionMessage(EnablePneumaticTransactionMessage msg) => IsPneumaticTransactionEnabled = msg.Value;

        private void OnReadStateMessage(ReadLinearLinkStateMessage msg) => msg.Read(this);

        private void OnUpdateStateMessage(UpdateLinearLinkStateMessage msg) => msg.Update(this);

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

        private void UpdateValue(MoveLinearLinkMessage msg)
        {
            msg.Update(this);
            if (msg.BackNotifyId > 0) MessengerInstance.Send(new BackNotificationMessage() { DestinationId = msg.BackNotifyId });
        }

        private void OnMoveLinearLinkMessage(MoveLinearLinkMessage msg)
        {
            if (Id == msg.LinkId)
            {
                if (IsPneumaticTransactionEnabled)
                {
                    _backNotifyId = msg.BackNotifyId;
                    LinearLinkMovementManager.Add(msg.LinkId, Value, msg.Value, msg.Duration);
                }
                else
                {
                    UpdateValue(msg);
                }
            }
        }

        private void OnLinearInterpolationLinkMessage(LinearInterpolationLinkMessage msg)
        {
            if(Id == msg.LinkId)
            {
                if(IsPneumaticTransactionEnabled)
                {
                    _backNotifyId = msg.BackNotifyId;
                    LinearLinkMovementManager.Add(msg.GroupId, msg.LinkId, Value, msg.Value, msg.Duration);
                }
                else
                {
                    UpdateValue(msg);
                }
            }
        }

        private void OnArcInterpolationLinkMessage(ArcInterpolationLinkMessage msg)
        {
            if (Id == msg.LinkId)
            {
                if (IsPneumaticTransactionEnabled)
                {
                    _backNotifyId = msg.BackNotifyId;
                    LinearLinkMovementManager.Add(msg.LinkId, msg.Value, msg.Duration, msg.ArcComponentData);
                }
                else
                {
                    UpdateValue(msg);
                }
            }
        }

        private void OnUpdateLinearLinkStateToTargetMessage(UpdateLinearLinkStateToTargetMessage msg)
        {
            if (Id == msg.LinkId)
            {
                msg.Update(this);
                if (msg.IsCompleted && (_backNotifyId > 0))
                {
                    MessengerInstance.Send(new BackNotificationMessage() { DestinationId = _backNotifyId });
                    _backNotifyId = 0;
                }
            }
        }


        private void OnCheckLinkPresenceMessage(CheckLinkPresenceMessage msg) => msg.NotifyAction?.Invoke();


        private void OnReadLinkLimitsMessage(ReadLinkLimitsMessage msg)
        {
            if(msg.LinkId == Id) msg.SetLimits?.Invoke(Min, Max);
        }


        protected override void OnValueChanged()
        {
            base.OnValueChanged();
            if (_gantryData != null) _gantryData.SetSlaveValue(Value);
            if (!IsPneumaticTransactionEnabled) LinearLinkMovementManager.ForceMaterialRemoval();
        }

        public void SetGantrySlave(IUpdatableValueLink<double> slave)
        {
            double offset = (slave != null) ? slave.Value - Value : 0.0;

            _gantryData = new GantryData()
            {
                Slave = slave,
                Offset = offset
            };
        }

        public void ResetGantry() => _gantryData = null;
    }
}
