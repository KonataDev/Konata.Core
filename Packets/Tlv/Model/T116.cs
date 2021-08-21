using System;

namespace Konata.Core.Packets.Tlv.Model
{
    public class T116Body : TlvBody
    {
        public readonly byte _ver;
        public readonly uint _bitmap;
        public readonly uint _getSig;
        public readonly uint[] _subAppIdList;

        public T116Body(uint bitmap, uint getSig, uint[] subAppIdList)
            : base()
        {
            _ver = 0;
            _bitmap = bitmap;
            _getSig = getSig;
            _subAppIdList = subAppIdList;

            PutByte(_ver);
            PutUintBE(_bitmap);
            PutUintBE(_getSig);

            PutByte((byte)_subAppIdList.Length);
            {
                foreach (uint element in _subAppIdList)
                {
                    PutUintBE(element);
                }
            }
        }

        public T116Body(byte[] data)
            : base(data)
        {
            TakeByte(out _ver);
            TakeUintBE(out _bitmap);
            TakeUintBE(out _getSig);

            TakeByte(out var count);
            {
                _subAppIdList = new uint[count];
                for (int i = 0; i < count; ++i)
                {
                    TakeUintBE(out _subAppIdList[i]);
                }
            }
        }
    }
}
