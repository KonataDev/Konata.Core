using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T521 : TlvBase
    {
        private readonly uint _productType;
        private readonly ushort _unknown;

        public T521(uint productType = 0, ushort unknown = 0)
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
            builder.PutUintBE(_productType);
            builder.PutUshortBE(_unknown);
            return builder.GetBytes();
        }
    }
}