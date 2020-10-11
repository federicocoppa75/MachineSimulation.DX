using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CncViewer.Connection.Messages
{
    class ValueChangedMessage<T>
    {
        public int LinkId { get; set; }
        public T Value { get; set; }
    }
}
