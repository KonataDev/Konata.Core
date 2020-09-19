using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{    
    public class T536 : TlvBase
    {
        private readonly byte[] _loginExtraData;

        public T536(byte[] loginExtraData) : base()
        {
            _loginExtraData = loginExtraData;
            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x536);
        }

        public override void PutTlvBody()
        {
            PutBytes(_loginExtraData);
        }
    }
}
