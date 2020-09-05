using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T191 : TlvBase
    {
        private readonly int _unknownK;

        public T191(int unknownK = 0x82)
        {
            _unknownK = unknownK;
        }

        public override ushort GetTlvCmd()
        {
            return 0x191;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushUInt8((byte)_unknownK);
            return builder.GetBytes();
        }
    }
}
