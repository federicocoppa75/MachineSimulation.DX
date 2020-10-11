using CncViewer.Connection.Enums;
using System;

namespace CncViewer.Connection.Messages
{
    class GetVariableToReadMessage
    {
        public Action<int, LinkType, string> AddVariableAct { get; set; }
    }
}
