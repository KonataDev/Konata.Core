using System;

namespace Konata.Library.IO
{
    public enum Endian
    {
        Big,
        Little
    }

    public static class ByteConverter
    {
        public static Endian DefaultEndian { get; set; } = BitConverter.IsLittleEndian ? Endian.Little : Endian.Big;

        public static byte[] BoolToBytes(bool value, uint length, Endian endian)
        {
            return endian == Endian.Little ? BoolToBytesLE(value, length) : BoolToBytesBE(value, length);
        }

        public static byte[] BoolToBytes(bool value, uint length)
        {
            return BoolToBytes(value, length, DefaultEndian);
        }

        public static byte[] BoolToBytesLE(bool value, uint length)
        {
            byte[] result = new byte[length];
            if (value)
            {
                result[0] = 1;
            }
            return result;
        }

        public static byte[] BoolToBytesBE(bool value, uint length)
        {
            byte[] result = new byte[length];
            if (value)
            {
                result[length - 1] = 1;
            }
            return result;
        }

        [Obsolete]
        public static byte[] Int8ToBytes(sbyte value, Endian endian)
        {
            return new byte[] { (byte)value };
        }

        public static byte[] Int8ToBytes(sbyte value)
        {
            return new byte[] { (byte)value };
        }

        [Obsolete]
        public static byte[] Int8ToBytesLE(sbyte value)
        {
            return new byte[] { (byte)value };
        }

        [Obsolete]
        public static byte[] Int8ToBytesBE(sbyte value)
        {
            return new byte[] { (byte)value };
        }

        [Obsolete]
        public static byte[] UInt8ToBytes(byte value, Endian endian)
        {
            return new byte[] { value };
        }

        public static byte[] UInt8ToBytes(byte value)
        {
            return new byte[] { value };
        }

        [Obsolete]
        public static byte[] UInt8ToBytesLE(byte value)
        {
            return new byte[] { value };
        }

        [Obsolete]
        public static byte[] UInt8ToBytesBE(byte value)
        {
            return new byte[] { value };
        }

        public static byte[] Int16ToBytes(short value, Endian endian)
        {
            return endian == Endian.Little ?
                Int16ToBytesLE(value) :
                Int16ToBytesBE(value);
        }

        public static byte[] Int16ToBytes(short value)
        {
            return Int16ToBytes(value, DefaultEndian);
        }

        public static byte[] Int16ToBytesLE(short value)
        {
            return new byte[]
            {
                (byte)(value & 0xFF),
                (byte)(value >> 8)
            };
        }

        public static byte[] Int16ToBytesBE(short value)
        {
            return new byte[]
            {
                (byte)(value >> 8),
                (byte)(value & 0xFF)
            };
        }

        public static byte[] UInt16ToBytes(ushort value, Endian endian)
        {
            return endian == Endian.Little ?
                UInt16ToBytesLE(value) :
                UInt16ToBytesBE(value);
        }

        public static byte[] UInt16ToBytes(ushort value)
        {
            return UInt16ToBytes(value, DefaultEndian);
        }

        public static byte[] UInt16ToBytesLE(ushort value)
        {
            return new byte[]
            {
                (byte)(value & 0xFF),
                (byte)(value >> 8)
            };
        }

        public static byte[] UInt16ToBytesBE(ushort value)
        {
            return new byte[]
            {
                (byte)(value >> 8),
                (byte)(value & 0xFF)
            };
        }

        public static byte[] Int32ToBytes(int value, Endian endian)
        {
            return endian == Endian.Little ?
                Int32ToBytesLE(value) :
                Int32ToBytesBE(value);
        }

        public static byte[] Int32ToBytes(int value)
        {
            return Int32ToBytes(value, DefaultEndian);
        }

        public static byte[] Int32ToBytesLE(int value)
        {
            return new byte[]
            {
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)(value >> 24)
            };
        }

        public static byte[] Int32ToBytesBE(int value)
        {
            return new byte[]
            {
                (byte)(value >> 24),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }

        public static byte[] UInt32ToBytes(uint value, Endian endian)
        {
            return endian == Endian.Little ?
                UInt32ToBytesLE(value) :
                UInt32ToBytesBE(value);
        }

        public static byte[] UInt32ToBytes(uint value)
        {
            return UInt32ToBytes(value, DefaultEndian);
        }

        public static byte[] UInt32ToBytesLE(uint value)
        {
            return new byte[]
            {
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)(value >> 24)
            };
        }

        public static byte[] UInt32ToBytesBE(uint value)
        {
            return new byte[]
            {
                (byte)(value >> 24),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }

        public static byte[] Int64ToBytes(long value, Endian endian)
        {
            return endian == Endian.Little ?
                Int64ToBytesLE(value) :
                Int64ToBytesBE(value);
        }

        public static byte[] Int64ToBytes(long value)
        {
            return Int64ToBytes(value, DefaultEndian);
        }

        public static byte[] Int64ToBytesLE(long value)
        {
            return new byte[]
            {
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 48) & 0xFF),
                (byte)(value >> 56)
            };
        }

        public static byte[] Int64ToBytesBE(long value)
        {
            return new byte[]
            {
                (byte)(value >> 56),
                (byte)((value >> 48) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }

        public static byte[] UInt64ToBytes(ulong value, Endian endian)
        {
            return endian == Endian.Little ?
                UInt64ToBytesLE(value) :
                UInt64ToBytesBE(value);
        }

        public static byte[] UInt64ToBytes(ulong value)
        {
            return UInt64ToBytes(value, DefaultEndian);
        }

        public static byte[] UInt64ToBytesLE(ulong value)
        {
            return new byte[]
            {
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 48) & 0xFF),
                (byte)(value >> 56)
            };
        }

        public static byte[] UInt64ToBytesBE(ulong value)
        {
            return new byte[]
            {
                (byte)(value >> 56),
                (byte)((value >> 48) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }

        public static byte[] IntToBytes(long value, uint length, Endian endian)
        {
            switch (length)
            {
                case 1: return Int8ToBytes((sbyte)value);
                case 2: return Int16ToBytes((short)value, endian);
                case 4: return Int32ToBytes((int)value, endian);
                case 8: return Int64ToBytes(value, endian);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static byte[] IntToBytes(long value, uint length)
        {
            switch (length)
            {
                case 1: return Int8ToBytes((sbyte)value);
                case 2: return Int16ToBytes((short)value);
                case 4: return Int32ToBytes((int)value);
                case 8: return Int64ToBytes(value);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static byte[] IntToBytesLE(long value, uint length)
        {
            switch (length)
            {
                case 1: return Int8ToBytes((sbyte)value);
                case 2: return Int16ToBytesLE((short)value);
                case 4: return Int32ToBytesLE((int)value);
                case 8: return Int64ToBytesLE(value);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static byte[] IntToBytesBE(long value, uint length)
        {
            switch (length)
            {
                case 1: return Int8ToBytes((sbyte)value);
                case 2: return Int16ToBytesBE((short)value);
                case 4: return Int32ToBytesBE((int)value);
                case 8: return Int64ToBytesBE(value);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static byte[] UIntToBytes(ulong value, uint length, Endian endian)
        {
            switch (length)
            {
                case 1: return UInt8ToBytes((byte)value);
                case 2: return UInt16ToBytes((ushort)value, endian);
                case 4: return UInt32ToBytes((uint)value, endian);
                case 8: return UInt64ToBytes(value, endian);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static byte[] UIntToBytes(ulong value, uint length)
        {
            switch (length)
            {
                case 1: return UInt8ToBytes((byte)value);
                case 2: return UInt16ToBytes((ushort)value);
                case 4: return UInt32ToBytes((uint)value);
                case 8: return UInt64ToBytes(value);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static byte[] UIntToBytesLE(ulong value, uint length)
        {
            switch (length)
            {
                case 1: return UInt8ToBytes((byte)value);
                case 2: return UInt16ToBytesLE((ushort)value);
                case 4: return UInt32ToBytesLE((uint)value);
                case 8: return UInt64ToBytesLE(value);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static byte[] UIntToBytesBE(ulong value, uint length)
        {
            switch (length)
            {
                case 1: return UInt8ToBytes((byte)value);
                case 2: return UInt16ToBytesBE((ushort)value);
                case 4: return UInt32ToBytesBE((uint)value);
                case 8: return UInt64ToBytesBE(value);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static bool BytesToBool(byte[] buffer, uint startIndex, uint length, Endian endian)
        {
            return endian == Endian.Little ?
                BytesToBoolLE(buffer, startIndex, length) :
                BytesToBoolBE(buffer, startIndex, length);
        }

        public static bool BytesToBool(byte[] buffer, uint startIndex, uint length)
        {
            return BytesToBool(buffer, startIndex, length, DefaultEndian);
        }

        public static bool BytesToBoolLE(byte[] buffer, uint startIndex, uint length)
        {
            return buffer[startIndex] > 0;
        }

        public static bool BytesToBoolBE(byte[] buffer, uint startIndex, uint length)
        {
            return buffer[startIndex + length - 1] > 0;
        }

        [Obsolete]
        public static sbyte BytesToInt8(byte[] buffer, uint startIndex, Endian endian)
        {
            return (sbyte)buffer[startIndex];
        }

        public static sbyte BytesToInt8(byte[] buffer, uint startIndex)
        {
            return (sbyte)buffer[startIndex];
        }

        [Obsolete]
        public static sbyte BytesToInt8LE(byte[] buffer, uint startIndex)
        {
            return (sbyte)buffer[startIndex];
        }

        [Obsolete]
        public static sbyte BytesToInt8BE(byte[] buffer, uint startIndex)
        {
            return (sbyte)buffer[startIndex];
        }

        [Obsolete]
        public static byte BytesToUInt8(byte[] buffer, uint startIndex, Endian endian)
        {
            return buffer[startIndex];
        }

        public static byte BytesToUInt8(byte[] buffer, uint startIndex)
        {
            return buffer[startIndex];
        }

        [Obsolete]
        public static byte BytesToUInt8LE(byte[] buffer, uint startIndex)
        {
            return buffer[startIndex];
        }

        [Obsolete]
        public static byte BytesToUInt8BE(byte[] buffer, uint startIndex)
        {
            return buffer[startIndex];
        }

        public static short BytesToInt16(byte[] buffer, uint startIndex, Endian endian)
        {
            return endian == Endian.Little ?
                BytesToInt16LE(buffer, startIndex) :
                BytesToInt16BE(buffer, startIndex);
        }

        public static short BytesToInt16(byte[] buffer, uint startIndex)
        {
            return BytesToInt16(buffer, startIndex, DefaultEndian);
        }

        public static short BytesToInt16LE(byte[] buffer, uint startIndex)
        {
            return (short)(buffer[startIndex] |
                (buffer[startIndex + 1] << 8));
        }

        public static short BytesToInt16BE(byte[] buffer, uint startIndex)
        {
            return (short)((buffer[startIndex] << 8) |
                buffer[startIndex + 1]);
        }

        public static ushort BytesToUInt16(byte[] buffer, uint startIndex, Endian endian)
        {
            return endian == Endian.Little ?
                BytesToUInt16LE(buffer, startIndex) :
                BytesToUInt16BE(buffer, startIndex);
        }

        public static ushort BytesToUInt16(byte[] buffer, uint startIndex)
        {
            return BytesToUInt16(buffer, startIndex, DefaultEndian);
        }

        public static ushort BytesToUInt16LE(byte[] buffer, uint startIndex)
        {
            return (ushort)(buffer[startIndex] |
                (buffer[startIndex + 1] << 8));
        }

        public static ushort BytesToUInt16BE(byte[] buffer, uint startIndex)
        {
            return (ushort)((buffer[startIndex] << 8) |
                buffer[startIndex + 1]);
        }

        public static int BytesToInt32(byte[] buffer, uint startIndex, Endian endian)
        {
            return endian == Endian.Little ?
                BytesToInt32LE(buffer, startIndex) :
                BytesToInt32BE(buffer, startIndex);
        }

        public static int BytesToInt32(byte[] buffer, uint startIndex)
        {
            return BytesToInt32(buffer, startIndex, DefaultEndian);
        }

        public static int BytesToInt32LE(byte[] buffer, uint startIndex)
        {
            return buffer[startIndex] |
                (buffer[startIndex + 1] << 8) |
                (buffer[startIndex + 2] << 16) |
                (buffer[startIndex + 3] << 24);
        }

        public static int BytesToInt32BE(byte[] buffer, uint startIndex)
        {
            return (buffer[startIndex] << 24) |
                (buffer[startIndex + 1] << 16) |
                (buffer[startIndex + 2] << 8) |
                buffer[startIndex + 3];
        }

        public static uint BytesToUInt32(byte[] buffer, uint startIndex, Endian endian)
        {
            return endian == Endian.Little ?
                BytesToUInt32LE(buffer, startIndex) :
                BytesToUInt32BE(buffer, startIndex);
        }

        public static uint BytesToUInt32(byte[] buffer, uint startIndex)
        {
            return BytesToUInt32(buffer, startIndex, DefaultEndian);
        }

        public static uint BytesToUInt32LE(byte[] buffer, uint startIndex)
        {
            return (uint)(buffer[startIndex] |
                (buffer[startIndex + 1] << 8) |
                (buffer[startIndex + 2] << 16) |
                (buffer[startIndex + 3] << 24));
        }

        public static uint BytesToUInt32BE(byte[] buffer, uint startIndex)
        {
            return (uint)((buffer[startIndex] << 24) |
                (buffer[startIndex + 1] << 16) |
                (buffer[startIndex + 2] << 8) |
                buffer[startIndex + 3]);
        }

        public static long BytesToInt64(byte[] buffer, uint startIndex, Endian endian)
        {
            return endian == Endian.Little ?
                BytesToInt64LE(buffer, startIndex) :
                BytesToInt64BE(buffer, startIndex);
        }

        public static long BytesToInt64(byte[] buffer, uint startIndex)
        {
            return BytesToInt64(buffer, startIndex, DefaultEndian);
        }

        public static long BytesToInt64LE(byte[] buffer, uint startIndex)
        {
            uint low = (uint)(buffer[startIndex] |
                (buffer[startIndex + 1] << 8) |
                (buffer[startIndex + 2] << 16) |
                (buffer[startIndex + 3] << 24));
            uint high = (uint)(buffer[startIndex + 4] |
                (buffer[startIndex + 5] << 8) |
                (buffer[startIndex + 6] << 16) |
                (buffer[startIndex + 7] << 24));
            return ((long)high << 32) | low;
        }

        public static long BytesToInt64BE(byte[] buffer, uint startIndex)
        {
            uint high = (uint)((buffer[startIndex] << 24) |
                (buffer[startIndex + 1] << 16) |
                (buffer[startIndex + 2] << 8) |
                buffer[startIndex + 3]);
            uint low = (uint)((buffer[startIndex + 4] << 24) |
                (buffer[startIndex + 5] << 16) |
                (buffer[startIndex + 6] << 8) |
                buffer[startIndex + 7]);
            return ((long)high << 32) | low;
        }

        public static ulong BytesToUInt64(byte[] buffer, uint startIndex, Endian endian)
        {
            return endian == Endian.Little ?
                BytesToUInt64LE(buffer, startIndex) :
                BytesToUInt64BE(buffer, startIndex);
        }

        public static ulong BytesToUInt64(byte[] buffer, uint startIndex)
        {
            return BytesToUInt64(buffer, startIndex, DefaultEndian);
        }

        public static ulong BytesToUInt64LE(byte[] buffer, uint startIndex)
        {
            uint low = (uint)(buffer[startIndex] |
                (buffer[startIndex + 1] << 8) |
                (buffer[startIndex + 2] << 16) |
                (buffer[startIndex + 3] << 24));
            uint high = (uint)(buffer[startIndex + 4] |
                (buffer[startIndex + 5] << 8) |
                (buffer[startIndex + 6] << 16) |
                (buffer[startIndex + 7] << 24));
            return ((ulong)high << 32) | low;
        }

        public static ulong BytesToUInt64BE(byte[] buffer, uint startIndex)
        {
            uint high = (uint)((buffer[startIndex] << 24) |
                (buffer[startIndex + 1] << 16) |
                (buffer[startIndex + 2] << 8) |
                buffer[startIndex + 3]);
            uint low = (uint)((buffer[startIndex + 4] << 24) |
                (buffer[startIndex + 5] << 16) |
                (buffer[startIndex + 6] << 8) |
                buffer[startIndex + 7]);
            return ((ulong)high << 32) | low;
        }

        public static long BytesToInt(byte[] buffer, uint startIndex, uint length, Endian endian)
        {
            switch (length)
            {
                case 1: return BytesToInt8(buffer, startIndex);
                case 2: return BytesToInt16(buffer, startIndex, endian);
                case 4: return BytesToInt32(buffer, startIndex, endian);
                case 8: return BytesToInt64(buffer, startIndex, endian);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static long BytesToInt(byte[] buffer, uint startIndex, uint length)
        {
            switch (length)
            {
                case 1: return BytesToInt8(buffer, startIndex);
                case 2: return BytesToInt16(buffer, startIndex);
                case 4: return BytesToInt32(buffer, startIndex);
                case 8: return BytesToInt64(buffer, startIndex);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static long BytesToIntLE(byte[] buffer, uint startIndex, uint length)
        {
            switch (length)
            {
                case 1: return BytesToInt8(buffer, startIndex);
                case 2: return BytesToInt16LE(buffer, startIndex);
                case 4: return BytesToInt32LE(buffer, startIndex);
                case 8: return BytesToInt64LE(buffer, startIndex);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static long BytesToIntBE(byte[] buffer, uint startIndex, uint length)
        {
            switch (length)
            {
                case 1: return BytesToInt8(buffer, startIndex);
                case 2: return BytesToInt16BE(buffer, startIndex);
                case 4: return BytesToInt32BE(buffer, startIndex);
                case 8: return BytesToInt64BE(buffer, startIndex);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static ulong BytesToUInt(byte[] buffer, uint startIndex, uint length, Endian endian)
        {
            switch (length)
            {
                case 1: return BytesToUInt8(buffer, startIndex);
                case 2: return BytesToUInt16(buffer, startIndex, endian);
                case 4: return BytesToUInt32(buffer, startIndex, endian);
                case 8: return BytesToUInt64(buffer, startIndex, endian);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static ulong BytesToUInt(byte[] buffer, uint startIndex, uint length)
        {
            switch (length)
            {
                case 1: return BytesToUInt8(buffer, startIndex);
                case 2: return BytesToUInt16(buffer, startIndex);
                case 4: return BytesToUInt32(buffer, startIndex);
                case 8: return BytesToUInt64(buffer, startIndex);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static ulong BytesToUIntLE(byte[] buffer, uint startIndex, uint length)
        {
            switch (length)
            {
                case 1: return BytesToUInt8(buffer, startIndex);
                case 2: return BytesToUInt16LE(buffer, startIndex);
                case 4: return BytesToUInt32LE(buffer, startIndex);
                case 8: return BytesToUInt64LE(buffer, startIndex);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static ulong BytesToUIntBE(byte[] buffer, uint startIndex, uint length)
        {
            switch (length)
            {
                case 1: return BytesToUInt8(buffer, startIndex);
                case 2: return BytesToUInt16BE(buffer, startIndex);
                case 4: return BytesToUInt32BE(buffer, startIndex);
                case 8: return BytesToUInt64BE(buffer, startIndex);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
