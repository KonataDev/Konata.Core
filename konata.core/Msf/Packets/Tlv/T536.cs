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

        public override ushort GetTlvCmd()
        {
            return 0x536;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushBytes(_loginExtraData, false);
            return builder.GetBytes();
        }
    }
}
