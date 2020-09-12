using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T107 : TlvBase
    {
        private readonly ushort _picType;
        private readonly byte _capType;
        private readonly ushort _picSize;
        private readonly byte _retType;

        public T107(int picType, int capType = 0, int picSize = 0, int retType = 1)
        {
            _picType = (ushort)picType;
            _capType = (byte)capType;
            _picSize = (ushort)picSize;
            _retType = (byte)retType;
        }

        public override ushort GetTlvCmd()
        {
            return 0x107;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUshortBE(_picType);
            builder.PutByte(_capType);
            builder.PutUshortBE(_picSize);
            builder.PutByte(_retType);
            return builder.GetBytes();
        }
    }
}