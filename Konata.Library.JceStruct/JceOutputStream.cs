using System;
using System.Linq;
using System.Collections.Generic;
using Konata.Library.IO;

namespace Konata.Library.JceStruct
{
    public class JceOutputStream : ByteBuffer
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
            if (index >= 0xF)
            {
                PutByte((byte)((int)type | 0xF0));
                PutByte(index);
            }
            else
            {
                byte t = (byte)((int)type | (index << 4));
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
            else if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
            {
                PutJceTypeHeader(JceType.Byte, index);
                PutSbyte((sbyte)value);
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
            PutFloatBE(value);
        }

        public void Write(double value, byte index)
        {
            if (value == 0)
            {
                PutJceZeroTag(index);
                return;
            }

            PutJceTypeHeader(JceType.Double, index);
            PutDoubleBE(value);
        }

        public void Write(string value, byte index)
        {
            if (value.Length <= byte.MaxValue)
            {
                PutJceTypeHeader(JceType.String1, index);
                PutString(value, Prefix.Uint8);
            }
            else
            {
                PutJceTypeHeader(JceType.String4, index);
                PutString(value, Prefix.Uint32);
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
            switch (value)
            {
            case byte @byte: Write(@byte, index); break;
            case sbyte @sbyte: Write(@sbyte, index); break;
            case short @short: Write(@short, index); break;
            case ushort @ushort: Write(@ushort, index); break;
            case int @int: Write(@int, index); break;
            case uint @uint: Write(@uint, index); break;
            case long @long: Write(@long, index); break;
            case ulong @ulong: Write(@ulong, index); break;
            case string @string: Write(@string, index); break;
            case byte[] bytes: Write(bytes, index); break;
            case ByteBuffer buffer: Write(buffer.GetBytes(), index); break;
            default: throw new Exception("Not supported Jce Type.");
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

        private void PutJceZeroTag(byte index)
        {
            PutJceTypeHeader(JceType.ZeroTag, index);
        }
    }
}
