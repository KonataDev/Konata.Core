using System;
using System.IO;
using System.Linq;
using System.Text;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Utils.IO;

/// <summary>
/// 字节流缓冲
/// </summary>
internal class ByteBuffer
{
    /// <summary>
    /// 报文前缀类型
    /// </summary>
    public enum Prefix
    {
        None = 0,
        Uint8 = 0b0001,
        Uint16 = 0b0010,
        Uint32 = 0b0100,
        /// <summary>
        /// 仅包含数据长度
        /// </summary>
        LengthOnly = 0,
        /// <summary>
        /// 前缀+数据总长度
        /// </summary>
        WithPrefix = 0b1000,
    }

    private static readonly IOException eobException =
        new IOException("Insufficient buffer space");

    private static int minBufferBase = 8;
    private static uint minBufferSize = 1U << minBufferBase;

    protected byte[] buffer;
    protected uint bufferLength;
    protected uint readPosition;
    protected uint writePosition;

    public ByteBuffer(byte[] data = null)
    {
        buffer = null;
        bufferLength = 0;
        readPosition = 0;
        writePosition = 0;
        if (data != null)
        {
            bufferLength = (uint)data.Length;
            WriteData(data);
        }
    }

    #region PutMethods 放入數據 此方法組會增長緩衝區

    public void PutSbyte(sbyte value)
    {
        WriteData(ByteConverter.Int8ToBytes(value));
    }

    public void PutByte(byte value)
    {
        WriteData(ByteConverter.UInt8ToBytes(value));
    }

    /// <summary>
    /// 以 Big Endian 字节序, 放入 short 
    /// </summary>
    /// <param name="value">值</param>
    public void PutShortBE(short value)
    {
        WriteData(ByteConverter.Int16ToBytes(value, Endian.Big));
    }

    /// <summary>
    /// 以 Little Endian 字节序, 放入 short 
    /// </summary>
    /// <param name="value">值</param>
    public void PutShortLE(short value)
    {
        WriteData(ByteConverter.Int16ToBytes(value, Endian.Little));
    }

    /// <summary>
    /// 放入 short 
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="endian">字节序</param>
    public void PutShort(short value, Endian endian)
    {
        WriteData(ByteConverter.Int16ToBytes(value, endian));
    }

    /// <summary>
    /// 以 Big Endian 字节序, 放入 ushort 
    /// </summary>
    /// <param name="value">值</param>
    public void PutUshortBE(ushort value)
    {
        WriteData(ByteConverter.UInt16ToBytes(value, Endian.Big));
    }

    /// <summary>
    /// 以 Little Endian 字节序, 放入 ushort 
    /// </summary>
    /// <param name="value">值</param>
    public void PutUshortLE(ushort value)
    {
        WriteData(ByteConverter.UInt16ToBytes(value, Endian.Little));
    }

    /// <summary>
    /// 放入 ushort 
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="endian">字节序</param>
    public void PutUshort(ushort value, Endian endian)
    {
        WriteData(ByteConverter.UInt16ToBytes(value, endian));
    }

    /// <summary>
    /// 以 Big Endian 字节序, 放入 int 
    /// </summary>
    /// <param name="value">值</param>
    public void PutIntBE(int value)
    {
        WriteData(ByteConverter.Int32ToBytes(value, Endian.Big));
    }

    /// <summary>
    /// 以 Little Endian 字节序, 放入 int 
    /// </summary>
    /// <param name="value">值</param>
    public void PutIntLE(int value)
    {
        WriteData(ByteConverter.Int32ToBytes(value, Endian.Little));
    }

    /// <summary>
    /// 放入 int 
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="endian">字节序</param>
    public void PutInt(int value, Endian endian)
    {
        WriteData(ByteConverter.Int32ToBytes(value, endian));
    }

    /// <summary>
    /// 以 Big Endian 字节序, 放入 uint 
    /// </summary>
    /// <param name="value">值</param>
    public void PutUintBE(uint value)
    {
        WriteData(ByteConverter.UInt32ToBytes(value, Endian.Big));
    }

    /// <summary>
    /// 以 Little Endian 字节序, 放入 uint 
    /// </summary>
    /// <param name="value">值</param>
    public void PutUintLE(uint value)
    {
        WriteData(ByteConverter.UInt32ToBytes(value, Endian.Little));
    }

    /// <summary>
    /// 放入 uint 
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="endian">字节序</param>
    public void PutUint(uint value, Endian endian)
    {
        WriteData(ByteConverter.UInt32ToBytes(value, endian));
    }

    /// <summary>
    /// 以 Big Endian 字节序, 放入 long 
    /// </summary>
    /// <param name="value">值</param>
    public void PutLongBE(long value)
    {
        WriteData(ByteConverter.Int64ToBytes(value, Endian.Big));
    }

    /// <summary>
    /// 以 Little Endian 字节序, 放入 long 
    /// </summary>
    /// <param name="value">值</param>
    public void PutLongLE(long value)
    {
        WriteData(ByteConverter.Int64ToBytes(value, Endian.Little));
    }

    /// <summary>
    /// 放入 long 
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="endian">字节序</param>
    public void PutLong(long value, Endian endian)
    {
        WriteData(ByteConverter.Int64ToBytes(value, endian));
    }

    /// <summary>
    /// 以 Big Endian 字节序, 放入 ulong 
    /// </summary>
    /// <param name="value">值</param>
    public void PutUlongBE(ulong value)
    {
        WriteData(ByteConverter.UInt64ToBytes(value, Endian.Big));
    }

    /// <summary>
    /// 以 Little Endian 字节序, 放入 ulong 
    /// </summary>
    /// <param name="value">值</param>
    public void PutUlongLE(ulong value)
    {
        WriteData(ByteConverter.UInt64ToBytes(value, Endian.Little));
    }

    /// <summary>
    /// 放入 ulong 
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="endian">字节序</param>
    public void PutUlong(ulong value, Endian endian)
    {
        WriteData(ByteConverter.UInt64ToBytes(value, endian));
    }

    /// <summary>
    /// 以 Big Endian 字节序, 放入 float 
    /// </summary>
    /// <param name="value">值</param>
    public void PutFloatBE(float value)
    {
        WriteData(ByteConverter.SingleToBytes(value, Endian.Big));
    }

    /// <summary>
    /// 以 Little Endian 字节序, 放入 float 
    /// </summary>
    /// <param name="value">值</param>
    public void PutFloatLE(float value)
    {
        WriteData(ByteConverter.SingleToBytes(value, Endian.Little));
    }

    /// <summary>
    /// 放入 float 
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="endian">字节序</param>
    public void PutFloat(float value, Endian endian)
    {
        WriteData(ByteConverter.SingleToBytes(value, endian));
    }

    /// <summary>
    /// 以 Big Endian 字节序, 放入 double 
    /// </summary>
    /// <param name="value">值</param>
    public void PutDoubleBE(double value)
    {
        WriteData(ByteConverter.DoubleToBytes(value, Endian.Big));
    }

    /// <summary>
    /// 以 Little Endian 字节序, 放入 double 
    /// </summary>
    /// <param name="value">值</param>
    public void PutDoubleLE(double value)
    {
        WriteData(ByteConverter.DoubleToBytes(value, Endian.Little));
    }

    /// <summary>
    /// 放入 double 
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="endian">字节序</param>
    public void PutDouble(double value, Endian endian)
    {
        WriteData(ByteConverter.DoubleToBytes(value, endian));
    }

    /// <summary>
    /// 以 Big Endian 字节序, 放入 bool
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="length">占用长度</param>
    public void PutBoolBE(bool value, byte length)
    {
        WriteData(ByteConverter.BoolToBytes(value, length, Endian.Big));
    }

    /// <summary>
    /// 以 Little Endian 字节序, 放入 bool
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="length">占用长度</param>
    public void PutBoolLE(bool value, byte length)
    {
        WriteData(ByteConverter.BoolToBytes(value, length, Endian.Little));
    }

    /// <summary>
    /// 放入 bool 
    /// </summary>
    /// <param name="value">值</param>
    /// <param name="length">长度</param>
    /// <param name="endian">字节序</param>
    public void PutBool(bool value, byte length, Endian endian)
    {
        WriteData(ByteConverter.BoolToBytes(value, length, endian));
    }

    /// <summary>
    /// 放入字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="prefixFlag"></param>
    /// <param name="limitedLength"></param>
    public void PutString(string value, Prefix prefixFlag = Prefix.None, byte limitedLength = 0)
    {
        PutBytes(Encoding.UTF8.GetBytes(value), prefixFlag, limitedLength); // 把字符串当作byte[]
    }

    /// <summary>
    /// 放入 Hex字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="prefixFlag"></param>
    /// <param name="limitedLength"></param>
    public void PutHexString(string value, Prefix prefixFlag = Prefix.None, byte limitedLength = 0)
    {
        var data = ByteConverter.UnHex(value);
        PutBytes(data, prefixFlag, limitedLength);
    }

    /// <summary>
    /// 放入 ByteBuffer
    /// </summary>
    /// <param name="value"></param>
    /// <param name="prefixFlag"></param>
    /// <param name="limitedLength"></param>
    public void PutByteBuffer(ByteBuffer value, Prefix prefixFlag = Prefix.None, byte limitedLength = 0)
    {
        PutBytes(value.GetBytes(), prefixFlag, limitedLength);
    }

    public void PutBytes(byte[] value, Prefix prefixFlag = Prefix.None, byte limitedLength = 0)
    {
        int prefixLength = (int)prefixFlag & 0b0111;
        bool limited = limitedLength > 0; // 是否限制长度
        byte[] array; // 处理后的数据
        if (limited) // 限制长度时，写入数据长度=前缀+限制
        {
            limitedLength = (byte)value.Length;
            array = new byte[prefixLength + limitedLength];
            int len = value.Length > limitedLength ? limitedLength : value.Length;
            if (len > 0)
            {
                Buffer.BlockCopy(value, 0, array, prefixLength, len);
            }
        }
        else if (prefixLength > 0) // 不限制长度且有前缀时，写入数据长度=前缀+value长度
        {
            array = new byte[prefixLength + value.Length];
            if (value.Length > 0)
            {
                Buffer.BlockCopy(value, 0, array, prefixLength, value.Length);
            }
        }
        else // 不限制又没有前缀，写入的就是value本身，不用处理，直接写入
        {
            WriteData(value);
            return;
        }
        if (prefixLength > 0) // 添加前缀，使用大端序
        {
            int len = value.Length;
            if ((prefixFlag & Prefix.WithPrefix) > 0)
            {
                len += prefixLength;
            }
            if (!InsertPrefix(array, 0, (uint)len, (Prefix)prefixLength))
            {
                throw new IOException("Given prefix length is too small for value bytes."); // 给定的prefix不够填充value.Length，终止写入
            }
        }
        WriteData(array);
    }

    public void PutEmpty(int count)
    {
        uint minLength = writePosition + (uint)count;
        ExtendBufferSize(minLength > bufferLength ? minLength : bufferLength);
        writePosition = minLength;
    }
    #endregion

    #region TakeMethods 拿走數據 此方法組會縮短緩衝區

    public sbyte TakeSbyte(out sbyte value)
    {
        if (CheckAvailable(1))
        {
            value = ByteConverter.BytesToInt8(buffer, readPosition);
            ++readPosition;
            return value;
        }
        throw eobException;
    }

    public byte TakeByte(out byte value)
    {
        if (CheckAvailable(1))
        {
            value = ByteConverter.BytesToUInt8(buffer, readPosition);
            ++readPosition;
            return value;
        }
        throw eobException;
    }

    public short TakeShortBE(out short value)
    {
        return TakeShort(out value, Endian.Big);
    }

    public short TakeShortLE(out short value)
    {
        return TakeShort(out value, Endian.Little);
    }

    public short TakeShort(out short value, Endian endian)
    {
        if (CheckAvailable(2))
        {
            value = ByteConverter.BytesToInt16(buffer, readPosition, endian);
            readPosition += 2;
            return value;
        }
        throw eobException;
    }

    public ushort TakeUshortBE(out ushort value)
    {
        return TakeUshort(out value, Endian.Big);
    }

    public ushort TakeUshortLE(out ushort value)
    {
        return TakeUshort(out value, Endian.Little);
    }

    public ushort TakeUshort(out ushort value, Endian endian)
    {
        if (CheckAvailable(2))
        {
            value = ByteConverter.BytesToUInt16(buffer, readPosition, endian);
            readPosition += 2;
            return value;
        }
        throw eobException;
    }

    public int TakeIntBE(out int value)
    {
        return TakeInt(out value, Endian.Big);
    }

    public int TakeIntLE(out int value)
    {
        return TakeInt(out value, Endian.Little);
    }

    public int TakeInt(out int value, Endian endian)
    {
        if (CheckAvailable(4))
        {
            value = ByteConverter.BytesToInt32(buffer, readPosition, endian);
            readPosition += 4;
            return value;
        }
        throw eobException;
    }

    public uint TakeUintBE(out uint value)
    {
        return TakeUint(out value, Endian.Big);
    }

    public uint TakeUintLE(out uint value)
    {
        return TakeUint(out value, Endian.Little);
    }

    public uint TakeUint(out uint value, Endian endian)
    {
        if (CheckAvailable(4))
        {
            value = ByteConverter.BytesToUInt32(buffer, readPosition, endian);
            readPosition += 4;
            return value;
        }
        throw eobException;
    }

    public long TakeLongBE(out long value)
    {
        return TakeLong(out value, Endian.Big);
    }

    public long TakeLongLE(out long value)
    {
        return TakeLong(out value, Endian.Little);
    }

    public long TakeLong(out long value, Endian endian)
    {
        if (CheckAvailable(8))
        {
            value = ByteConverter.BytesToInt64(buffer, readPosition, endian);
            readPosition += 8;
            return value;
        }
        throw eobException;
    }

    public ulong TakeUlongBE(out ulong value)
    {
        return TakeUlong(out value, Endian.Big);
    }

    public ulong TakeUlongLE(out ulong value)
    {
        return TakeUlong(out value, Endian.Little);
    }

    public ulong TakeUlong(out ulong value, Endian endian)
    {
        if (CheckAvailable(8))
        {
            value = ByteConverter.BytesToUInt64(buffer, readPosition, endian);
            readPosition += 8;
            return value;
        }
        throw eobException;
    }

    public float TakeFloatBE(out float value)
    {
        return TakeFloat(out value, Endian.Big);
    }

    public float TakeFloatLE(out float value)
    {
        return TakeFloat(out value, Endian.Little);
    }

    public float TakeFloat(out float value, Endian endian)
    {
        if (CheckAvailable(4))
        {
            value = ByteConverter.BytesToSingle(buffer, readPosition, endian);
            readPosition += 4;
            return value;
        }
        throw eobException;
    }

    public double TakeDoubleBE(out double value)
    {
        return TakeDouble(out value, Endian.Big);
    }

    public double TakeDoubleLE(out double value)
    {
        return TakeDouble(out value, Endian.Little);
    }

    public double TakeDouble(out double value, Endian endian)
    {
        if (CheckAvailable(8))
        {
            value = ByteConverter.BytesToDouble(buffer, readPosition, endian);
            readPosition += 8;
            return value;
        }
        throw eobException;
    }

    public bool TakeBoolBE(out bool value, byte length)
    {
        return TakeBool(out value, length, Endian.Big);
    }

    public bool TakeBoolLE(out bool value, byte length)
    {
        return TakeBool(out value, length, Endian.Little);
    }

    public bool TakeBool(out bool value, byte length, Endian endian)
    {
        if (CheckAvailable(length))
        {
            value = ByteConverter.BytesToBool(buffer, readPosition, length, endian);
            readPosition += length;
            return value;
        }
        throw eobException;
    }

    public string TakeString(out string value, Prefix prefixFlag)
    {
        return value = Encoding.UTF8.GetString(TakeBytes(out byte[] _, prefixFlag));
    }

    public byte[] TakeBytes(out byte[] value, Prefix prefixFlag)
    {
        uint length;
        bool reduce = (prefixFlag & Prefix.WithPrefix) > 0;
        uint preLen = ((uint)prefixFlag) & 0b0111;
        switch (preLen)
        {
            case 0: // Read to end.
                length = RemainLength;
                break;
            case 1:
            case 2:
            case 4:
                if (CheckAvailable(preLen))
                {
                    length = preLen == 1 ? ByteConverter.BytesToUInt8(buffer, readPosition) :
                             preLen == 2 ? ByteConverter.BytesToUInt16(buffer, readPosition, Endian.Big) :
                                           ByteConverter.BytesToUInt32(buffer, readPosition, Endian.Big);
                    readPosition += preLen;
                    if (reduce)
                    {
                        if (length < preLen)
                        {
                            throw new IOException("Data length is less than prefix length.");
                        }
                        length -= preLen;
                    }
                    break;
                }
                throw eobException;
            default:
                throw new ArgumentOutOfRangeException("Invalid prefix flag.");
        }
        if (CheckAvailable(length))
        {
            value = new byte[length];
            Buffer.BlockCopy(buffer, (int)readPosition, value, 0, (int)length);
            readPosition += length;
            return value;
        }
        throw eobException;
    }

    public byte[] TakeBytes(out byte[] value, uint length)
    {
        if (CheckAvailable(length))
        {
            value = new byte[length];
            Buffer.BlockCopy(buffer, (int)readPosition, value, 0, (int)length);
            readPosition += length;
            return value;
        }
        throw eobException;
    }

    public byte[] TakeAllBytes(out byte[] value)
    {
        value = new byte[bufferLength - readPosition];
        Buffer.BlockCopy(buffer, (int)readPosition, value, 0, value.Length);
        readPosition = bufferLength;
        return value;
    }

    /// <summary>
    /// 吃掉數據 φ(゜▽゜*)♪
    /// </summary>
    /// <param name="length"></param>
    public void EatBytes(uint length)
    {
        if (CheckAvailable(length))
        {
            readPosition += length;
        }
        else
        {
            throw eobException;
        }
    }

    public byte[] TakeVarIntBytes(out byte[] value)
    {
        value = null;
        uint index = readPosition;
        byte b;
        do
        {
            if (index < bufferLength)
            {
                b = buffer[index];
                ++index;
            }
            else
            {
                throw eobException;
            }
        }
        while ((b & 0b10000000) > 0);
        return value = TakeBytes(out _, index - readPosition);
    }

    public ulong TakeVarIntValueBE(out ulong value)
    {
        value = 0;
        byte b;
        do
        {
            if (CheckAvailable(1))
            {
                TakeByte(out b);
                value <<= 7;
                value |= b & 0b01111111u;
            }
            else
            {
                throw eobException;
            }
        }
        while ((b & 0b10000000) > 0);
        return value;
    }

    public ulong TakeVarIntValueLE(out ulong value)
    {
        value = 0;
        int count = 0;
        byte b;
        do
        {
            if (CheckAvailable(1))
            {
                TakeByte(out b);
                value |= (b & 0b01111111u) << (count * 7);
                ++count;
            }
            else
            {
                throw eobException;
            }
        }
        while ((b & 0b10000000) > 0);
        return value;
    }

    #endregion

    #region GetMethods 獲取數據 此方法組不會對緩衝區造成影響

    /// <summary>
    /// 獲取數據
    /// </summary>
    /// <returns></returns>
    public byte[] GetBytes()
    {
        if (bufferLength > 0)
        {
            var data = new byte[bufferLength];
            Buffer.BlockCopy(buffer, 0, data, 0, (int)bufferLength);
            return data;
        }
        return new byte[0];
    }

    /// <summary>
    /// 到字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return ByteConverter.Hex(PeekBytes(RemainLength, out var _), true);
    }

    #endregion

    #region PeekMethods 查看數據 此方法組不會對緩衝區造成影響

    public byte PeekByte(uint offset, out byte value)
    {
        if (CheckAvailable(offset + 1))
        {
            value = ByteConverter.BytesToUInt8(buffer, readPosition + offset);
            return value;
        }
        throw eobException;
    }

    public byte PeekByte(out byte value)
    {
        return PeekByte(0, out value);
    }

    public int PeekInt(uint offset, out int value, Endian endian)
    {
        if (CheckAvailable(offset + 4))
        {
            value = ByteConverter.BytesToInt32(buffer, readPosition + offset, endian);
            return value;
        }
        throw eobException;
    }

    public uint PeekUint(uint offset, out uint value, Endian endian)
    {
        if (CheckAvailable(offset + 4))
        {
            value = ByteConverter.BytesToUInt32(buffer, readPosition + offset, endian);
            return value;
        }
        throw eobException;
    }

    public uint PeekUshort(uint offset, out ushort value, Endian endian)
    {
        if (CheckAvailable(offset + 2))
        {
            value = ByteConverter.BytesToUInt16(buffer, readPosition + offset, endian);
            return value;
        }
        throw eobException;
    }

    public int PeekIntBE(out int value)
    {
        return PeekInt(0, out value, Endian.Big);
    }

    public int PeekIntBE(uint offset, out int value)
    {
        return PeekInt(offset, out value, Endian.Big);
    }

    public uint PeekUintBE(out uint value)
    {
        return PeekUint(0, out value, Endian.Big);
    }

    public uint PeekUintBE(uint offset, out uint value)
    {
        return PeekUint(offset, out value, Endian.Big);
    }

    public uint PeekUintLE(uint offset, out uint value)
    {
        return PeekUint(offset, out value, Endian.Little);
    }

    public uint PeekUshortBE(uint offset, out ushort value)
    {
        return PeekUshort(offset, out value, Endian.Big);
    }

    public uint PeekUshortLE(uint offset, out ushort value)
    {
        return PeekUshort(offset, out value, Endian.Little);
    }

    public byte[] PeekBytes(uint offset, uint length, out byte[] value)
    {
        if (CheckAvailable(offset + length))
        {
            value = new byte[length];
            Array.Copy(buffer, readPosition + offset, value, 0, length);
            return value;
        }
        throw eobException;
    }

    public byte[] PeekBytes(uint length, out byte[] value)
    {
        return PeekBytes(0, length, out value);
    }

    #endregion

    /// <summary>
    /// 缓冲区总长度
    /// </summary>
    public uint Length
    {
        get { return bufferLength; }
    }

    /// <summary>
    /// 缓冲区读指针到末尾长度
    /// </summary>
    public uint RemainLength
    {
        get { return bufferLength - readPosition; }
    }

    /// <summary>
    /// 最小缓冲
    /// </summary>
    public static uint MinBufferBase
    {
        get
        {
            return (uint)minBufferBase;
        }
        set
        {
            if (value > 30)
            {
                throw new IOException($"最小缓冲过大(0-30,当前值{value})");
            }
            minBufferBase = (int)value;
            minBufferSize = 1U << minBufferBase;
        }
    }

    /// <summary>
    /// 最小缓冲
    /// </summary>
    public static uint MinBufferSize
    {
        get
        {
            return minBufferSize;
        }
        set
        {
            if (value == 0 || value > 0x40000000)
            {
                throw new IOException($"最小缓冲过大(0-0x40000000,当前值{(Int32)value})");
            }
            int b = 0;
            uint s = 1;
            while (s < value)
            {
                ++b;
                s <<= 1;
            }
            minBufferBase = b;
            minBufferSize = s;
        }
    }

    private void ExtendBufferSize(uint minLength)
    {
        uint size = minLength >> minBufferBase;
        if ((minLength & (minBufferSize - 1)) > 0)
        {
            ++size;
        }
        size <<= minBufferBase;
        if (buffer == null)
        {
            buffer = new byte[size];
        }
        else if (buffer.Length < size)
        {
            Array.Resize(ref buffer, (int)size);
        }
        if (bufferLength < minLength)
        {
            bufferLength = minLength;
        }
    }

    /// <summary>
    /// 将新数据写入缓冲区
    /// </summary>
    /// <param name="data">字节数组</param>
    protected void WriteData(byte[] data)
    {
        uint minLength = writePosition + (uint)data.Length;
        ExtendBufferSize(minLength > bufferLength ? minLength : bufferLength);
        Buffer.BlockCopy(data, 0, buffer, (int)writePosition, data.Length);
        writePosition = minLength;
    }

    protected bool CheckAvailable(uint length = 0)
    {
        return readPosition + length <= bufferLength;
    }

    protected static bool InsertPrefix(byte[] buffer, uint offset, uint value, Prefix prefixFlag, Endian endian = Endian.Big)
    {
        switch (prefixFlag)
        {
            case Prefix.Uint8:
                if (value <= byte.MaxValue)
                {
                    Buffer.BlockCopy(ByteConverter.UInt8ToBytes((byte)value), 0, buffer, (int)offset, 1);
                    return true;
                }
                break;
            case Prefix.Uint16:
                if (value <= ushort.MaxValue)
                {
                    Buffer.BlockCopy(ByteConverter.UInt16ToBytes((ushort)value, endian), 0, buffer, (int)offset, 2);
                    return true;
                }
                break;
            case Prefix.Uint32:
                Buffer.BlockCopy(ByteConverter.UInt32ToBytes(value, endian), 0, buffer, (int)offset, 4);
                return true;
        }
        return false;
    }

    #region Operators

    public static ByteBuffer operator +(ByteBuffer a, ByteBuffer b)
    {
        var buffer = new ByteBuffer();
        buffer.PutBytes(a.GetBytes());
        buffer.PutBytes(b.GetBytes());
        return buffer;
    }

    public static ByteBuffer operator +(byte[] a, ByteBuffer b)
    {
        var buffer = new ByteBuffer();
        buffer.PutBytes(a);
        buffer.PutBytes(b.GetBytes());
        return buffer;
    }

    public static ByteBuffer operator +(ByteBuffer a, byte[] b)
    {
        var buffer = new ByteBuffer();
        buffer.PutBytes(a.GetBytes());
        buffer.PutBytes(b);
        return buffer;
    }

    public static bool operator ==(ByteBuffer a, ByteBuffer b)
    {
        return a is null ? b is null : a.Equals(b);
    }

    public static bool operator !=(ByteBuffer a, ByteBuffer b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        ByteBuffer other = (ByteBuffer)obj;
        return !(other is null)
            && Length == other.Length
            && (buffer == null
            || other.buffer == null
            || Enumerable.SequenceEqual(buffer, other.buffer));
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}
