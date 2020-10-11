using CncViewer.Connection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CncViewer.ConnectionTester.Implementation
{
    class BoolVariableValueChangedObserver : IVariableValueChangedObserver<bool>
    {
        public void ValueChanged(int linkId, bool value)
        {
        }
    }
}
