using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T521 : TlvBase
    {
        private readonly int _productType;
        private readonly short _unknown;

        public T521(int productType = 0, short unknown = 0)
        {
            _productType = productType;
            _unknown = unknown;
        }

        public override ushort GetTlvCmd()
        {
            return 0x521;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushInt32(_productType);
            builder.PushInt16(_unknown);
            return builder.GetBytes();
        }
    }
}