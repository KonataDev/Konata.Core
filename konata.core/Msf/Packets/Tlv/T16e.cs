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
            return 0x16e;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutString(_deviceName, 2);
            return builder.GetBytes();
        }
    }
}
