using CncViewer.Connection.CncInterface;
using CncViewer.Connection.Messages;
using CncViewer.Models.Connection.Enums;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Timers;

namespace CncViewer.Connection.Helpers
{
    public class VariableReadingEngine
    {

        static VariableReadingEngine _instance;
        static bool _firstRead = true;

        Timer _timer;

        Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();

        public VariableReadingEngine()
        {
            Messenger.Default.Register<StartReadingMessage>(this, OnStartReadingMessage);
            Messenger.Default.Register<StopReadingMessage>(this, OnStopReadingMessage);
        }

        public static void InitializeInstance()
        {
            _instance = new VariableReadingEngine();
        }

        private void OnStopReadingMessage(StopReadingMessage msg)
        {
            _timer.Enabled = false;
            _timer.Elapsed -= OnElapsed;
            _timer.Dispose();
            _timer = null;

            ResetComunication();
        }


        private void OnStartReadingMessage(StartReadingMessage msg)
        {
            InitializeComunication(msg.ChennelType);

            _timer = new Timer(300);
            _timer.Elapsed += OnElapsed;
            _timer.AutoReset = false;
            _timer.Enabled = true;

            _firstRead = true;
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Enabled = false;

            foreach (var item in _variables.Values) item.Read();

            if(_firstRead)
            {
                _firstRead = false;
                Messenger.Default.Send(new GetAllValuesMessage());
            }
            _timer.Enabled = true;
        }

        private void InitializeComunication(ChannelType channelType)
        {
            string channel = GetChannel(channelType);
            //int iChannel = KvCom3x.ConvComunicationChannel("SIMULATO");
            //int iChannel = KvCom3x.ConvComunicationChannel("NETWORK");
            int iChannel = KvCom3x.ConvComunicationChannel(channel);
            int iError = KvCom3x.init_board("defcn", iChannel);

            if (iError != 0)
            {
                string error = KvCom3x.GetKvComErrorMsg(iError);
            }

            InitializeVariables();
        }

        private string GetChannel(ChannelType channelType)
        {
            switch (channelType)
            {
                case ChannelType.Simulato:
                    return "SIMULATO";
                case ChannelType.Network:
                    return "NETWORK";
                default:
                    throw new ArgumentException();
            }
        }

        private void InitializeVariables()
        {
            _variables.Clear();

            Messenger.Default.Send(new GetVariableToReadMessage()
            {
                AddVariableAct = (id, type, name) => AddVariavle(id, type, name)
            });
        }

        private void AddVariavle(int id, Enums.LinkType type, string name)
        {
            if (string.IsNullOrEmpty(name)) return;

            switch (type)
            {
                case Enums.LinkType.Linear:
                    _variables.Add(name, AxVariable.Create(id, name));
                    break;

                case Enums.LinkType.TwoPos:
                case Enums.LinkType.WriteTwoPos:
                case Enums.LinkType.PulseTwoPos:

                    var baseName = BitSetVariable.GetBaseName(name, out int idx);

                    if (_variables.TryGetValue(baseName, out Variable var))
                    {
                        (var as BitSetVariable).AddBit(id, idx);
                    }
                    else
                    {
                        _variables.Add(baseName, BitSetVariable.Create(id, baseName, idx));
                    }
                    break;

                default:
                    throw new ArgumentException("Invalid link type!");
            }
        }

        private void ResetComunication()
        {
            KvCom3x.exit_board();

            ResetVariable();
        }

        private void ResetVariable()
        {
            foreach (var item in _variables.Values) item.Reset();

            _variables.Clear();
        }
    }
}
