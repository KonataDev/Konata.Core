using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T154 : TlvBase
    {
        private readonly uint _ssoSequenceId;

        public T154(uint ssoSequenceId)
        {
            _ssoSequenceId = ssoSequenceId;
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
