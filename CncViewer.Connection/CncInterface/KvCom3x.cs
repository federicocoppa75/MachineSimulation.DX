using System;
using System.Runtime.InteropServices;

namespace CncViewer.Connection.CncInterface
{
    class KvCom3x
    {
        [DllImport(@"KvCom3x.dll")]
        public static extern string GetKvComErrorMsg(int iErr);

        [DllImport(@"KvCom3x.dll")]
        public static extern int ConvComunicationChannel(string sChannelType);

        [DllImport(@"KvCom3x.dll")]
        public static extern int init_board(string defcn_name, int ChannelType);

        [DllImport(@"KvCom3x.dll")]
        public static extern int exit_board();

        [DllImport(@"KvCom3x.dll")]
        public static extern int get_reg_by_name(string name, IntPtr pReg);

        [DllImport(@"KvCom3x.dll")]
        public static extern int read_regdword(IntPtr pReg, ushort Offset, IntPtr dwpVal);

        [DllImport(@"KvCom3x.dll")]
        public static extern int write_regdword(IntPtr pReg, ushort Offset, int val);
    }
}
