using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{    
    public class T536 : TlvBase
    {
        private readonly byte[] _loginExtraData;

        public T536(byte[] loginExtraData)
        {
            _loginExtraData = loginExtraData;
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
