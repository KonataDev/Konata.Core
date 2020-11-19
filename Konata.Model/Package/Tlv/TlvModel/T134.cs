using System;

namespace Konata.Model.Package.Tlv.TlvModel
{
    public class T134Body : TlvBody
    {
        public readonly byte[] _wtSessionTicketKey;

        public T134Body(byte[] wtSessionTicketKey, object nil)
            : base()
        {
            _wtSessionTicketKey = wtSessionTicketKey;

            PutBytes(_wtSessionTicketKey);
        }

        public T134Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _wtSessionTicketKey, Prefix.None);
        }
    }
}
