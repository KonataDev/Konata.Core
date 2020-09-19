using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    /// <summary>
    /// 未完成
    /// </summary>
    public class T544 : TlvBase
    {
        private readonly string _wtLoginSdk;

        public T544(string wtLoginSdk) : base()
        {
            _wtLoginSdk = wtLoginSdk;
            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x544);
        }

        public override void PutTlvBody()
        {

        }
    }
}
