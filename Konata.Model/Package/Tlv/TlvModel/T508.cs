using System;

namespace Konata.Model.Package.Tlv.TlvModel
{
    public class T508Body : TlvBody
    {
        public readonly bool _doFetch;
        public readonly uint _timeout;
        public readonly byte[] _userData;

        public T508Body(bool doFetch, uint timeout, byte[] userData)
            : base()
        {
            _doFetch = doFetch;
            _timeout = timeout;
            _userData = userData;

            PutBoolBE(_doFetch, 1);
            PutUintBE(_timeout);
            PutBytes(_userData);
        }

        public T508Body(byte[] data)
            : base(data)
        {
            TakeBoolBE(out _doFetch, 1);
            TakeUintBE(out _timeout);
            TakeBytes(out _userData, Prefix.Uint16);
        }
    }
}
