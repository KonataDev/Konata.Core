using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T521 : TlvBase
    {
        private readonly uint _productType;
        private readonly ushort _unknown;

        public T521(uint productType = 0, ushort unknown = 0) : base()
        {
            _productType = productType;
            _unknown = unknown;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x521);
        }

        public override void PutTlvBody()
        {
            PutUintBE(_productType);
            PutUshortBE(_unknown);
        }
    }
}
