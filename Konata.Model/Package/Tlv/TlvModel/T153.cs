using System;

namespace Konata.Model.Package.Tlv.TlvModel
{
    public class T153Body : TlvBody
    {
        public readonly bool _isRooted;

        public T153Body(bool isRooted)
            : base()
        {
            _isRooted = isRooted;

            PutBoolBE(_isRooted, 2);
        }

        public T153Body(byte[] data)
            : base(data)
        {
            TakeBoolBE(out _isRooted, 2);
        }
    }
}
