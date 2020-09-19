using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T116 : TlvBase
    {
        private const byte _ver = 0;

        private readonly uint _bitmap;
        private readonly uint _getSig;
        private readonly long[] _subAppIdList;

        public T116(int bitmap, int getSig, long[] subAppIdList = null) : base()
        {
            if (subAppIdList == null)
            {
                _subAppIdList = new long[] { 1600000226L };
            }

            _bitmap = (uint)bitmap;
            _getSig = (uint)getSig;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x116);
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutByte(_ver);
            builder.PutUintBE(_bitmap);
            builder.PutUintBE(_getSig);
            builder.PutByte((byte)_subAppIdList.Length);
            foreach (long element in _subAppIdList)
            {
                builder.PutUintBE((uint)element);
            }

            PutBytes(builder.GetBytes());
        }
    }
}
