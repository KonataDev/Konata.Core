using System;

namespace Konata.Model.Package.Tlv.TlvModel
{
    public class T11fBody : TlvBody
    {
        public readonly uint _chgTime;
        public readonly uint _tkPri;

        public T11fBody(uint chgTime, uint tkPri)
            : base()
        {
            _chgTime = chgTime;
            _tkPri = tkPri;

            PutUintBE(_chgTime);
            PutUintBE(_tkPri);
        }

        public T11fBody(byte[] data)
            : base(data)
        {
            TakeUintBE(out _chgTime);
            TakeUintBE(out _tkPri);
            EatBytes(2);
        }
    }
}
