using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T107 : TlvBase
    {
        private readonly ushort _picType;
        private readonly byte _capType;
        private readonly ushort _picSize;
        private readonly byte _retType;

        public T107(int picType, int capType = 0, int picSize = 0, int retType = 1) : base()
        {
            _picType = (ushort)picType;
            _capType = (byte)capType;
            _picSize = (ushort)picSize;
            _retType = (byte)retType;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x107);
        }

        public override void PutTlvBody()
        {
            PutUshortBE(_picType);
            PutByte(_capType);
            PutUshortBE(_picSize);
            PutByte(_retType);
        }
    }
}