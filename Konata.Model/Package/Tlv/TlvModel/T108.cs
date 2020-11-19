using System;

namespace Konata.Model.Package.Tlv.TlvModel
{
    public class T108Body : TlvBody
    {
        public readonly byte[] _ksid;

        public T108Body(byte[] ksid, int ksidLength)
            : base()
        {
            _ksid = ksid;

            PutBytes(_ksid);
        }

        public T108Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _ksid, Prefix.None);
        }
    }
}
