using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    /// <summary>
    /// <TODO>未完成</TODO>
    /// </summary>

    public class T104 : TlvBase
    {
        private readonly string _sigSession;

        public T104(string sigSession) : base()
        {
            _sigSession = sigSession;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x104);
        }

        public override void PutTlvBody()
        {

        }
    }
}
