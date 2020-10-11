using CncViewer.Connection.Messages;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace CncViewer.Connection.Helpers
{
    class BitSetVariable : Variable
    {
        Dictionary<int, int> _linkBitMap = new Dictionary<int, int>();
        object _lockValue = new object();

        public BitSetVariable()
        {
            Messenger.Default.Register<WriteValueMessage<bool>>(this, OnWriteValueMessage);
        }

        protected override void OnRead(int value)
        {
            lock (_lockValue)
            {
                if (Value != value)
                {
                    int chg = Value ^ value;

                    foreach (var id in _linkBitMap.Keys)
                    {
                        int mask = 1 << _linkBitMap[id];

                        if ((chg & mask) != 0)
                        {
                            var v = (value & mask) != 0;
                            Messenger.Default.Send(new ValueChangedMessage<bool>() { LinkId = id, Value = v });
                        }
                    }

                    Value = value;
                }
            }
        }

        public void AddBit(int linkId, int bitIndex) => _linkBitMap.Add(linkId, bitIndex);

        public static BitSetVariable Create(int linkId, string name, int bitIndex)
        {
            var v = new BitSetVariable();
            var iErr = v.Initialize(name);

            v.AddBit(linkId, bitIndex);

            return v;
        }

        public static string GetBaseName(string name, out int index)
        {
            char[] token = { '.' };
            var ss  = name.Split(token);
            var idx = int.Parse(ss.Last());
            var baseName = name.Replace($".{idx}", "");

            index = idx;

            return baseName;
        }


        private void OnWriteValueMessage(WriteValueMessage<bool> msg)
        {
            if(_linkBitMap.TryGetValue(msg.LinkId, out int bitIndex))
            {
                lock (_lockValue)
                {
                    int value = Value;
                    int mask = 1 << bitIndex;
                    bool bitVal = (value & mask) != 0;

                    if (bitVal != msg.Value)
                    {
                        if (msg.Value)
                        {
                            value |= mask;
                        }
                        else
                        {
                            value &= ~mask;
                        }

                        Write(value);
                    }
                }
            }
        }
    }
}
