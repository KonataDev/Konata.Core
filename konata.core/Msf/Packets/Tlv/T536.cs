using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
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
            return 0x536;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutBytes(_loginExtraData, 2);
            return builder.GetBytes();
        }
    }
}
