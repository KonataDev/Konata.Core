using System;
using System.Text;

namespace Konata.Core.Utils.IO;

internal enum Endian
{
    /// <summary>
    /// 大端
    /// </summary>
    Big,

    /// <summary>
    /// 小端
    /// </summary>
    Little,

    /// <summary>
    /// 随运行时主机
    /// </summary>
    FollowMachine,
}

/// <summary>
/// 值类型转byte工具
/// </summary>
internal static class ByteConverter
{
    public static Endian DefaultEndian { get; set; }
        = BitConverter.IsLittleEndian ? Endian.Little : Endian.Big;

    #region Value2Bytes

    /// <summary>
    /// 布尔转bytes
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="length">长度</param>
    /// <param name="endian">高低位,默认跟随主机配置</param>
    /// <returns></returns>
    public static byte[] BoolToBytes(bool value, uint length, Endian endian = Endian.FollowMachine)
    {
        if ((int) endian == (int) Endian.FollowMachine)
        {
            endian = DefaultEndian;
        }

        byte[] result = new byte[length];
        if (value)
        {
            result[endian == Endian.Little ? 0 : length - 1] = 1;
        }

        return result;
    }

    public static byte[] Int8ToBytes(sbyte value)
    {
        return new byte[] {(byte) value};
    }

    public static byte[] UInt8ToBytes(byte value)
    {
        return new byte[] {value};
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

    #endregion

    #region Bytes2Value

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
        return (sbyte) buffer[startIndex];
    }

    public static sbyte BytesToInt8(byte[] buffer, uint startIndex)
    {
        return (sbyte) buffer[startIndex];
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
            return BitConverter.ToInt16(buffer, (int) startIndex);
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
            return BitConverter.ToUInt16(buffer, (int) startIndex);
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
            return BitConverter.ToInt32(buffer, (int) startIndex);
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
            return BitConverter.ToUInt32(buffer, (int) startIndex);
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
            return BitConverter.ToInt64(buffer, (int) startIndex);
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
            return BitConverter.ToUInt64(buffer, (int) startIndex);
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
            return BitConverter.ToSingle(buffer, (int) startIndex);
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
            return BitConverter.ToDouble(buffer, (int) startIndex);
        }
    }

    public static double BytesToDouble(byte[] buffer, uint startIndex)
    {
        return BytesToDouble(buffer, startIndex, DefaultEndian);
    }

    #endregion

    private static bool ShouldReverse(Endian endian)
    {
        return BitConverter.IsLittleEndian ^ (endian == Endian.Little);
    }

    private static byte[] PickBytes(in byte[] buffer, uint startIndex, int length)
    {
        byte[] temp = new byte[length];
        Buffer.BlockCopy(buffer, (int) startIndex, temp, 0, length);
        Array.Reverse(temp);
        return temp;
    }

#pragma warning disable CS0675
    public static long VarintToNumber(in byte[] varint)
    {
        var number = 0L;

        for (int i = varint.Length - 1; i >= 0; --i)
        {
            number <<= 7;
            number |= varint[i] & 0b01111111;
        }

        return number;
    }
#pragma warning restore CS0675

    public static byte[] NumberToVarint(long number)
    {
        byte[] buffer;

        if (number >= 127)
        {
            var len = 0;
            buffer = new byte[10];

            do
            {
                buffer[len] = (byte) ((number & 127) | 128);
                number >>= 7;
                ++len;
            } while (number > 127);

            buffer[len] = (byte) number;
            Array.Resize(ref buffer, len + 1);
        }
        else
        {
            buffer = new byte[1] {(byte) number};
        }

        return buffer;
    }

    public static string Hex(in byte[] data, bool space = false)
    {
        return BitConverter.ToString(data).Replace("-", space ? " " : "");
    }

    public static byte[] UnHex(string hex)
    {
        hex = hex.Replace(" ", "");

        int length = hex.Length;
        byte[] bytes = new byte[length / 2];

        for (int i = 0; i < length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }

        return bytes;
    }

    public static string Base64(byte[] data)
        => Convert.ToBase64String(data, Base64FormattingOptions.None);

    public static byte[] UnBase64(string base64)
        => Convert.FromBase64String(base64);

    public static string UnBase64String(string base64)
        => Encoding.UTF8.GetString(UnBase64(base64));
}
