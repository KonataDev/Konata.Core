using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T154 : TlvBase
    {
        private readonly uint _ssoSequenceId;

        public T154(uint ssoSequenceId) : base()
        {
            _ssoSequenceId = ssoSequenceId;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x154);
        }

        public override void PutTlvBody()
        {
            PutUintBE(_ssoSequenceId);
        }
    }
}
