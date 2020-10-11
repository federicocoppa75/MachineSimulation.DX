using CncViewer.Connection.CncInterface;
using System;
using System.Runtime.InteropServices;

namespace CncViewer.Connection.Helpers
{
    abstract class Variable
    {
        IntPtr _regPtr;
        IntPtr _valuePtr;
        protected string _name;

        public int Value { get; protected set; }

        protected int Initialize(string name)
        {
            _name = name;
            _regPtr = Marshal.AllocHGlobal(Marshal.SizeOf<TS_REG>());
            _valuePtr = Marshal.AllocHGlobal(Marshal.SizeOf<int>());

            return KvCom3x.get_reg_by_name(name, _regPtr);
        }

        public void Read()
        {
            var iError = KvCom3x.read_regdword(_regPtr, 0, _valuePtr);
            var value = Marshal.PtrToStructure<int>(_valuePtr);

            OnRead(value);
        }

        public void Write() => Write(Value);

        public void Write(int value)
        {
            var iError = KvCom3x.write_regdword(_regPtr, 0, value);
        }

        public void Reset()
        {
            Marshal.FreeHGlobal(_regPtr);
            Marshal.FreeHGlobal(_valuePtr);
        }

        protected abstract void OnRead(int value);
    }

}
