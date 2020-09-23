using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T154Body : TlvBody
    {
        public readonly uint _ssoSequenceId;

        public T154Body(uint ssoSequenceId)
            : base()
        {
            _ssoSequenceId = ssoSequenceId;

            PutUintBE(_ssoSequenceId);
        }

        public T154Body(byte[] data)
            : base(data)
        {
            TakeUintBE(out _ssoSequenceId);
        }
    }
}
