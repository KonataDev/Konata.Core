using Konata.Msf;
using System;
using System.Linq;

namespace Konata.Utils
{
    public enum JceType : byte
    {
        Byte = 0,
        Short = 1,
        Int = 2,
        Long = 3,
        Float = 4,
        Double = 5,
        String1 = 6,
        String4 = 7,
        Map = 8,
        List = 9,
        StructBegin = 10,
        StructEnd = 11,
        ZeroTag = 12,
        SimpleList = 13
    }

    public class JceOutputStream : Packet
    {
        public JceOutputStream()
            : base()
        {

        }

        public JceOutputStream(JceOutputStream writer)
            : base(writer.GetBytes())
        {

        }

        private void PutJceTypeHeader(byte index, JceType type)
        {
            if (index >= 15)
            {
                byte t = (byte)((byte)type | 240);
                PutByte(t);
                PutByte(index);
            }
            else
            {
                byte t = (byte)((byte)type | (index << 4));
                PutByte(t);
            }
        }

        public void PutJceByte(byte index, byte value)
        {
            if (value == 0)
            {
                PutJceZeroTag(index);
                return;
            }

            PutJceTypeHeader(index, JceType.Byte);
            PutByte(value);
        }

        public void PutJceShort(byte index, short value)
        {
            if (value == 0)
            {
                PutJceZeroTag(index);
                return;
            }

            PutJceTypeHeader(index, JceType.Short);
            PutShortBE(value);
        }

        public void PutJceInt(byte index, int value)
        {
            if (value == 0)
            {
                PutJceZeroTag(index);
                return;
            }

            PutJceTypeHeader(index, JceType.Int);
            PutIntBE(value);
        }

        public void PutJceLong(byte index, long value)
        {
            if (value == 0)
            {
                PutJceZeroTag(index);
                return;
            }

            PutJceTypeHeader(index, JceType.Long);
            PutLongBE(value);
        }

        public void PutJceFloat(byte index, float value)
        {
            if (value == 0)
            {
                PutJceZeroTag(index);
                return;
            }

            PutJceTypeHeader(index, JceType.Float);
            PutBytes(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void PutJceDouble(byte index, double value)
        {
            if (value == 0)
            {
                PutJceZeroTag(index);
                return;
            }

            PutJceTypeHeader(index, JceType.Double);
            PutBytes(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void PutJceString(byte index, string value)
        {
            if (value.Length > 255)
            {
                PutJceTypeHeader(index, JceType.String1);
                PutString(value, 1);
            }
            else
            {
                PutJceTypeHeader(index, JceType.String4);
                PutString(value, 4);
            }
        }

        public void PutJceWriter(byte index, JceOutputStream value)
        {
            PutJceTypeHeader(index, JceType.String4);

        }

        //public void PutJceMap()
        //{

        //}

        //public void PutJceList()
        //{

        //}

        private void PutJceSimpleList()
        {

        }

        private void PutJceStructBegin()
        {

        }

        private void PutJceStructEnd()
        {

        }

        private void PutJceZeroTag(byte index)
        {
            PutJceTypeHeader(index, JceType.ZeroTag);
        }

    }
}
