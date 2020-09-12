using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T107 : TlvBase
    {
        private readonly short _picType;
        private readonly sbyte _capType;
        private readonly short _picSize;
        private readonly sbyte _retType;

        public T107(int picType, int capType = 0, int picSize = 0, int retType = 1)
        {
            _picType = (short)picType;
            _capType = (sbyte)capType;
            _picSize = (short)picSize;
            _retType = (sbyte)retType;
        }

        public override ushort GetTlvCmd()
        {
            return 0x107;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUshortBEBE(_picType);
            builder.PutInt8(_capType);
            builder.PutUshortBEBE(_picSize);
            builder.PutInt8(_retType);
            return builder.GetBytes();
        }
    }
}