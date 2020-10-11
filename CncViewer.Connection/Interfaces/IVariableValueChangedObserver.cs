using System;
using System.Collections.Generic;
using System.Text;

namespace CncViewer.Connection.Interfaces
{
    public interface IVariableValueChangedObserver<T>
    {
        void ValueChanged(int linkId, T value);
    }
}
