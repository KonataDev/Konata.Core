using System;
using System.Linq;
using System.Text;
using Konata.Library.IO;

namespace Konata.Library.JceStruct
{
    internal static class JceUtils
    {
        internal static byte UnTag(ByteBuffer buffer, uint offset,
            out JceType type, out byte tag)
        {
            var data = buffer.PeekByte(offset, out var _);

            tag = (byte)((data & 240) >> 4);
            type = (JceType)(data & 15);

            if (tag == 15)
            {
                tag = buffer.PeekByte(offset + 1, out var _);
                return 2;
            }

            return 1;
        }

        internal static byte[] Tag(JceType type, byte index)
        {
            byte[] buffer;

            if (index >= 0xF)
            {
                buffer = new byte[] { (byte)((int)type | 0xF0), index };
            }
            else
            {
                buffer = new byte[] { (byte)((int)type | (index << 4)) };
            }

            return buffer;
        }

        internal static uint UnJceStandardType(ByteBuffer buffer, uint offset, out JceType type,
            out byte tag, out byte[] data)
        {
            data = null;
            uint length = UnTag(buffer, offset, out type, out tag);

            switch (type)
            {
                case JceType.ZeroTag:
                    data = null;
                    length += 0;
                    break;
                case JceType.Byte:
                    buffer.PeekBytes(offset + length, 1, out data);
                    length += 1;
                    break;
                case JceType.Short:
                    buffer.PeekBytes(offset + length, 2, out data);
                    length += 2;
                    break;
                case JceType.Int:
                    buffer.PeekBytes(offset + length, 4, out data);
                    length += 4;
                    break;
                case JceType.Long:
                    buffer.PeekBytes(offset + length, 8, out data);
                    length += 8;
                    break;
                case JceType.Float:
                    buffer.PeekBytes(offset + length, 4, out data);
                    length += 4;
                    break;
                case JceType.Double:
                    buffer.PeekBytes(offset + length, 8, out data);
                    length += 8;
                    break;
                case JceType.String1:
                    buffer.PeekByte(offset + length, out var str1len);
                    buffer.PeekBytes(offset + length + 1, str1len, out data);
                    length += (uint)str1len + 1;
                    break;
                case JceType.String4:
                    buffer.PeekIntBE(offset + length, out var str4len);
                    buffer.PeekBytes(offset + length + 4, (uint)str4len, out data);
                    length += (uint)str4len + 4;
                    break;
            }

            return length;
        }

        internal static uint JceToNumber(ByteBuffer buffer, out JceType type,
            out byte tag, out long value)
        {
            value = 0;
            var length = UnJceStandardType(buffer, 0, out type, out tag, out var data);

            switch (type)
            {
                case JceType.ZeroTag:
                    value = 0;
                    break;
                case JceType.Byte:
                    value = ByteConverter.BytesToInt8(data, 0);
                    break;
                case JceType.Short:
                    value = ByteConverter.BytesToInt16(data, 0);
                    break;
                case JceType.Int:
                    value = ByteConverter.BytesToInt32(data, 0);
                    break;
                case JceType.Long:
                    value = ByteConverter.BytesToInt64(data, 0);
                    break;
                default: throw new Exception("invalid data, not a number.");
            }

            return length;
        }

        internal static object JceToObject(byte[] buffer, Type type, out object value)
        {
            if (type == typeof(byte))
                return value = buffer[0];
            else if (type == typeof(sbyte))
                return value = ByteConverter.BytesToInt8(buffer, 0);
            else if (type == typeof(short))
                return value = ByteConverter.BytesToInt16(buffer, 0, Endian.Big);
            else if (type == typeof(ushort))
                return value = ByteConverter.BytesToUInt16(buffer, 0, Endian.Big);
            else if (type == typeof(int))
                return value = ByteConverter.BytesToInt32(buffer, 0, Endian.Big);
            else if (type == typeof(uint))
                return value = ByteConverter.BytesToUInt32(buffer, 0, Endian.Big);
            else if (type == typeof(long))
                return value = ByteConverter.BytesToInt64(buffer, 0, Endian.Big);
            else if (type == typeof(string))
                return value = Encoding.UTF8.GetString(buffer);
            else if (type == typeof(byte[]))
                return value = buffer;
            else throw new Exception($"not supported this type. {type}");
        }

        internal static byte[] NumberToJce(long value,
            out JceType type, out byte[] buffer)
        {
            if (value == 0)
            {
                type = JceType.ZeroTag;
                buffer = new byte[0];
            }
            else if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
            {
                type = JceType.Byte;
                buffer = ByteConverter.Int8ToBytes((sbyte)value);
            }
            else if (value >= short.MinValue && value <= short.MaxValue)
            {
                type = JceType.Short;
                buffer = ByteConverter.Int16ToBytes((short)value, Endian.Big);
            }
            else if (value >= int.MinValue && value <= int.MaxValue)
            {
                type = JceType.Int;
                buffer = ByteConverter.Int32ToBytes((int)value, Endian.Big);
            }
            else
            {
                type = JceType.Long;
                buffer = ByteConverter.Int64ToBytes(value, Endian.Big);
            }

            return buffer;
        }

        internal static byte[] StringToJce(string value,
            out JceType type, out byte[] buffer)
        {
            var str = Encoding.UTF8.GetBytes(value);
            var length = new byte[0];

            if (str.Length <= byte.MaxValue)
            {
                type = JceType.String1;
                length = new byte[] { (byte)str.Length };
            }
            else
            {
                type = JceType.String4;
                length = ByteConverter.Int32ToBytes(str.Length);
            }

            buffer = length.Concat(str).ToArray();

            return buffer;
        }

        internal static byte[] BytesToJce(byte[] value,
            out JceType type, out byte[] buffer)
        {
            var length = NumberToJce(value.Length, out var lengthType, out var _);
            var lengthTag = Tag(lengthType, 0);
            var headerTag = Tag(JceType.Byte, 0);

            type = JceType.SimpleList;
            buffer = headerTag.Concat(lengthTag)
                .Concat(length)
                .Concat(value)
                .ToArray();

            return buffer;
        }

        internal static byte[] ObjectToJce(object value,
            out JceType type, out byte[] buffer)
        {
            switch (value)
            {
                case byte @byte:
                    return NumberToJce(@byte, out type, out buffer);
                case sbyte @sbyte:
                    return NumberToJce(@sbyte, out type, out buffer);
                case short @short:
                    return NumberToJce(@short, out type, out buffer);
                case ushort @ushort:
                    return NumberToJce(@ushort, out type, out buffer);
                case int @int:
                    return NumberToJce(@int, out type, out buffer);
                case uint @uint:
                    return NumberToJce(@uint, out type, out buffer);
                case long @long:
                    return NumberToJce(@long, out type, out buffer);
                case string @string:
                    return StringToJce(@string, out type, out buffer);
                case byte[] bytes:
                    return BytesToJce(bytes, out type, out buffer);
                case ByteBuffer byteBuffer:
                    return BytesToJce(byteBuffer.GetBytes(), out type, out buffer);
                default: throw new Exception("Not supported Jce Type.");
            }
        }

        internal static byte[] FloatToJce(float value,
           out JceType type, out byte[] buffer)
        {
            type = JceType.Float;
            buffer = ByteConverter.SingleToBytes(value);

            return buffer;
        }

        internal static byte[] DoubleToJce(double value,
            out JceType type, out byte[] buffer)
        {
            type = JceType.Double;
            buffer = ByteConverter.DoubleToBytes(value);

            return buffer;
        }
    }
}
