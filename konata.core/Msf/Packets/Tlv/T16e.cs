using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T16e : TlvBase
    {
        private readonly string _deviceName;

        public T16e(string deviceName)
        {
            _deviceName = deviceName;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x16e);
        }

        public override void PutTlvBody()
        {
            PutString(_deviceName, 2);
        }
    }
}
