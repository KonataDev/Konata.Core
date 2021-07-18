using System;

namespace Konata.Core.Packets.Tlv.TlvModel
{
    public class T154Body : TlvBody
    {
        public readonly int _ssoSequence;

        public T154Body(int ssoSequenceId)
            : base()
        {
            _ssoSequence = ssoSequenceId;

            PutIntBE(_ssoSequence);
        }

        public T154Body(byte[] data)
            : base(data)
        {
            TakeIntBE(out _ssoSequence);
        }
    }
}
