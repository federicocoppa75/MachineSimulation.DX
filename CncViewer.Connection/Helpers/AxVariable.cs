using CncViewer.Connection.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace CncViewer.Connection.Helpers
{
    class AxVariable : Variable
    {
        public int LinkId { get; private set; }

        public AxVariable() : base()
        {
            Messenger.Default.Register<WriteValueMessage<double>>(this, OnWriteValueMessage);
        }

        protected int Initialize(int linkId, string name)
        {
            LinkId = linkId;

            return base.Initialize(name);
        }

        protected override void OnRead(int value)
        {
            if (Value != value)
            {
                Value = value;
                Messenger.Default.Send(new ValueChangedMessage<double>() { LinkId = LinkId, Value = (Value / 1000.0) });
            }
        }

        public static AxVariable Create(int linkId, string name)
        {
            var v = new AxVariable();
            var iErr = v.Initialize(linkId, name);

            return v;
        }

        private void OnWriteValueMessage(WriteValueMessage<double> msg)
        {
            if(LinkId == msg.LinkId)
            {
                var v = (int)(msg.Value * 1000);

                Value = v;
                Write();
            }
        }
    }
}
