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
            byte[] result = new byte[length];
            if (value)
            {
                result[endian == Endian.Little ? 0 : length - 1] = 1;
            }
            return result;
        }

        public static byte[] BoolToBytes(bool value, uint length)
        {
            return BoolToBytes(value, length, DefaultEndian);
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
        public static byte[] UInt8ToBytes(byte value, Endian endian)
        {
            return new byte[] { value };
        }

        public static byte[] UInt8ToBytes(byte value)
        {
            return new byte[] { value };
        }

        public static byte[] Int16ToBytes(short value, Endian endian)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (ShouldReverse(endian))
            {
                Array.Reverse(result);
            }
            return result;
        }

        public static byte[] Int16ToBytes(short value)
        {
            return Int16ToBytes(value, DefaultEndian);
        }

        public static byte[] UInt16ToBytes(ushort value, Endian endian)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (ShouldReverse(endian))
            {
                Array.Reverse(result);
            }
            return result;
        }

        public static byte[] UInt16ToBytes(ushort value)
        {
            return UInt16ToBytes(value, DefaultEndian);
        }

        public static byte[] Int32ToBytes(int value, Endian endian)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (ShouldReverse(endian))
            {
                Array.Reverse(result);
            }
            return result;
        }

        public static byte[] Int32ToBytes(int value)
        {
            return Int32ToBytes(value, DefaultEndian);
        }

        public static byte[] UInt32ToBytes(uint value, Endian endian)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (ShouldReverse(endian))
            {
                Array.Reverse(result);
            }
            return result;
        }

        public static byte[] UInt32ToBytes(uint value)
        {
            return UInt32ToBytes(value, DefaultEndian);
        }

        public static byte[] Int64ToBytes(long value, Endian endian)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (ShouldReverse(endian))
            {
                Array.Reverse(result);
            }
            return result;
        }

        public static byte[] Int64ToBytes(long value)
        {
            return Int64ToBytes(value, DefaultEndian);
        }

        public static byte[] UInt64ToBytes(ulong value, Endian endian)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (ShouldReverse(endian))
            {
                Array.Reverse(result);
            }
            return result;
        }

        public static byte[] UInt64ToBytes(ulong value)
        {
            return UInt64ToBytes(value, DefaultEndian);
        }

        public static byte[] SingleToBytes(float value, Endian endian)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (ShouldReverse(endian))
            {
                Array.Reverse(result);
            }
            return result;
        }

        public static byte[] SingleToBytes(float value)
        {
            return SingleToBytes(value, DefaultEndian);
        }

        public static byte[] DoubleToBytes(double value, Endian endian)
        {
            byte[] result = BitConverter.GetBytes(value);
            if (ShouldReverse(endian))
            {
                Array.Reverse(result);
            }
            return result;
        }

        public static byte[] DoubleToBytes(double value)
        {
            return DoubleToBytes(value, DefaultEndian);
        }

        public static bool BytesToBool(byte[] buffer, uint startIndex, uint length, Endian endian)
        {
            return buffer[endian == Endian.Little ? startIndex : startIndex + length - 1] > 0;
        }

        public static bool BytesToBool(byte[] buffer, uint startIndex, uint length)
        {
            return BytesToBool(buffer, startIndex, length, DefaultEndian);
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
        public static byte BytesToUInt8(byte[] buffer, uint startIndex, Endian endian)
        {
            return buffer[startIndex];
        }

        public static byte BytesToUInt8(byte[] buffer, uint startIndex)
        {
            return buffer[startIndex];
        }

        public static short BytesToInt16(byte[] buffer, uint startIndex, Endian endian)
        {
            if (ShouldReverse(endian))
            {
                return BitConverter.ToInt16(PickBytes(buffer, startIndex, 2), 0);
            }
            else
            {
                return BitConverter.ToInt16(buffer, (int)startIndex);
            }
        }

        public static short BytesToInt16(byte[] buffer, uint startIndex)
        {
            return BytesToInt16(buffer, startIndex, DefaultEndian);
        }

        public static ushort BytesToUInt16(byte[] buffer, uint startIndex, Endian endian)
        {
            if (ShouldReverse(endian))
            {
                return BitConverter.ToUInt16(PickBytes(buffer, startIndex, 2), 0);
            }
            else
            {
                return BitConverter.ToUInt16(buffer, (int)startIndex);
            }
        }

        public static ushort BytesToUInt16(byte[] buffer, uint startIndex)
        {
            return BytesToUInt16(buffer, startIndex, DefaultEndian);
        }

        public static int BytesToInt32(byte[] buffer, uint startIndex, Endian endian)
        {
            if (ShouldReverse(endian))
            {
                return BitConverter.ToInt32(PickBytes(buffer, startIndex, 4), 0);
            }
            else
            {
                return BitConverter.ToInt32(buffer, (int)startIndex);
            }
        }

        public static int BytesToInt32(byte[] buffer, uint startIndex)
        {
            return BytesToInt32(buffer, startIndex, DefaultEndian);
        }

        public static uint BytesToUInt32(byte[] buffer, uint startIndex, Endian endian)
        {
            if (ShouldReverse(endian))
            {
                return BitConverter.ToUInt32(PickBytes(buffer, startIndex, 4), 0);
            }
            else
            {
                return BitConverter.ToUInt32(buffer, (int)startIndex);
            }
        }

        public static uint BytesToUInt32(byte[] buffer, uint startIndex)
        {
            return BytesToUInt32(buffer, startIndex, DefaultEndian);
        }

        public static long BytesToInt64(byte[] buffer, uint startIndex, Endian endian)
        {
            if (ShouldReverse(endian))
            {
                return BitConverter.ToInt64(PickBytes(buffer, startIndex, 8), 0);
            }
            else
            {
                return BitConverter.ToInt64(buffer, (int)startIndex);
            }
        }

        public static long BytesToInt64(byte[] buffer, uint startIndex)
        {
            return BytesToInt64(buffer, startIndex, DefaultEndian);
        }

        public static ulong BytesToUInt64(byte[] buffer, uint startIndex, Endian endian)
        {
            if (ShouldReverse(endian))
            {
                return BitConverter.ToUInt64(PickBytes(buffer, startIndex, 8), 0);
            }
            else
            {
                return BitConverter.ToUInt64(buffer, (int)startIndex);
            }
        }

        public static ulong BytesToUInt64(byte[] buffer, uint startIndex)
        {
            return BytesToUInt64(buffer, startIndex, DefaultEndian);
        }

        public static float BytesToSingle(byte[] buffer, uint startIndex, Endian endian)
        {
            if (ShouldReverse(endian))
            {
                return BitConverter.ToSingle(PickBytes(buffer, startIndex, 4), 0);
            }
            else
            {
                return BitConverter.ToSingle(buffer, (int)startIndex);
            }
        }

        public static float BytesToSingle(byte[] buffer, uint startIndex)
        {
            return BytesToSingle(buffer, startIndex, DefaultEndian);
        }

        public static double BytesToDouble(byte[] buffer, uint startIndex, Endian endian)
        {
            if (ShouldReverse(endian))
            {
                return BitConverter.ToDouble(PickBytes(buffer, startIndex, 8), 0);
            }
            else
            {
                return BitConverter.ToDouble(buffer, (int)startIndex);
            }
        }

        public static double BytesToDouble(byte[] buffer, uint startIndex)
        {
            return BytesToDouble(buffer, startIndex, DefaultEndian);
        }

        private static bool ShouldReverse(Endian endian)
        {
            return BitConverter.IsLittleEndian ^ (endian == Endian.Little);
        }

        private static byte[] PickBytes(byte[] buffer, uint startIndex, int length)
        {
            byte[] temp = new byte[length];
            Buffer.BlockCopy(buffer, (int)startIndex, temp, 0, length);
            Array.Reverse(temp);
            return temp;
        }
    }
}
