using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    /// <summary>
    /// <TODO>未完成</TODO>
    /// </summary>

    public class T104 : TlvBase
    {
        private readonly string _sigSession;

        public T104(string sigSession)
        {
            _sigSession = sigSession;
        }

        public override ushort GetTlvCmd()
        {
            return 0x104;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            return builder.GetBytes();
        }
    }
}
