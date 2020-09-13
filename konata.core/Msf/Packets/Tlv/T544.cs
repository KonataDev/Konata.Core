using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
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

        public override void PutTlvCmd()
        {
            return 0x544;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            return builder.GetBytes();
        }
    }
}