using System;
using System.Collections.Generic;
using System.Linq;
using Konata.Msf;

namespace Konata.Utils.Jce
{
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

        public void PutJceTypeHeader(JceType type, byte index)
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

        public void Write(byte[] value, byte index)
        {
            PutJceTypeHeader(JceType.SimpleList, index);
            PutJceTypeHeader(JceType.Byte, 0);
            Write(value.Length, 0);
            PutBytes(value);
        }

        public void Write(long value, byte index)
        {
            if (value == 0)
            {
                PutJceZeroTag(index);
                return;
            }
            else if (value > sbyte.MinValue && value <= sbyte.MaxValue)
            {
                PutJceTypeHeader(JceType.Byte, index);
                PutByte((byte)value);
            }
            else if (value >= short.MinValue && value <= short.MaxValue)
            {
                PutJceTypeHeader(JceType.Short, index);
                PutShortBE((short)value);
            }
            else if (value >= int.MinValue && value <= int.MaxValue)
            {
                PutJceTypeHeader(JceType.Int, index);
                PutIntBE((int)value);
            }
            else
            {
                PutJceTypeHeader(JceType.Long, index);
                PutLongBE(value);
            }
        }

        public void Write(float value, byte index)
        {
            if (value == 0)
            {
                PutJceZeroTag(index);
                return;
            }

            PutJceTypeHeader(JceType.Float, index);
            PutBytes(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void Write(double value, byte index)
        {
            if (value == 0)
            {
                PutJceZeroTag(index);
                return;
            }

            PutJceTypeHeader(JceType.Double, index);
            PutBytes(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void Write(string value, byte index)
        {
            if (value.Length <= 255)
            {
                PutJceTypeHeader(JceType.String1, index);
                PutString(value, 1);
            }
            else
            {
                PutJceTypeHeader(JceType.String4, index);
                PutString(value, 4);
            }
        }

        public void Write<K, V>(Dictionary<K, V> value, byte index)
        {
            PutJceTypeHeader(JceType.Map, index);
            Write(value == null ? 0 : value.Count, 0);
            if (value != null)
            {
                foreach (var element in value)
                {
                    Write(element.Key, 0);
                    Write(element.Value, 1);
                }
            }
        }

        public void Write(object value, byte index)
        {
            if (value is byte || value is sbyte
                || value is short || value is ushort
                || value is int || value is uint
                || value is long || value is ulong)
            {
                Write((long)value, index);
            }
            else if (value is string)
            {
                Write((string)value, index);
            }
            else if (value is byte[])
            {
                Write((byte[])value, index);
            }
            else if (value is Packet)
            {
                Write(((Packet)value).GetBytes(), index);
            }
            else
            {
                throw new Exception("Not supported Jce Type.");
            }
        }


        public void Write(JceOutputStream value, byte index)
        {
            PutJceTypeHeader(JceType.StructBegin, 0);
            {
                Write(value.GetBytes(), 0);
            }
            PutJceTypeHeader(JceType.StructEnd, 1);
        }

        //public void PutJceMap()
        //{

        //}

        //public void PutJceList()
        //{

        //}


        private void PutJceZeroTag(byte index)
        {
            PutJceTypeHeader(JceType.ZeroTag, index);
        }

    }
}
