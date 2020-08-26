using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    /// <summary>
    /// 未完成
    /// </summary>
    public class T544 : TlvBase
    {
        private readonly string _wtLoginSdk;

        public T544(string wtLoginSdk)
        {
            _wtLoginSdk = wtLoginSdk;
        }

        public override ushort GetTlvCmd()
        {
            return 0x544;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            return builder.GetBytes();
        }
    }
}