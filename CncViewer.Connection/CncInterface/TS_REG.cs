namespace CncViewer.Connection.CncInterface
{
    public struct TS_REG
    {
        public byte board;          //
        public byte iBaseReg;       //
        public ushort type;           //
        public ulong iSharedId;      // id shared
        public ulong iNum;           //
        public ulong iMax;           //
        public ulong iSiz;           //
        public ulong iOff;           //
        public ulong ChkSum;         // checksum del contenuto della ts_reg
    }
}
