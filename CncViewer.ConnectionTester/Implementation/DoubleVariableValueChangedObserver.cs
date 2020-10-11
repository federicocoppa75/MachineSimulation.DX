using CncViewer.Connection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CncViewer.ConnectionTester.Implementation
{
    class DoubleVariableValueChangedObserver : IVariableValueChangedObserver<double>
    {
        public void ValueChanged(int linkId, double value)
        {
        }
    }
}
