using System;
using System.IO;
using System.Text;

namespace Konata.Library.IO
{
    public class ByteBuffer
    {
        public enum Prefix
        {
            None = 0, // 前綴類型
            Uint8 = 1,
            Uint16 = 2,
            Uint32 = 4,

            LengthOnly = 64, // 只包括數據長度
            WithPrefix = 128,  // 包括數據長度和前綴長度
        }

        protected byte[] _buffer;
        protected uint _length;
        private static readonly IOException _bufferErr =
            new IOException("Insufficient buffer space.");

        public ByteBuffer()
        {

        }

        public ByteBuffer(byte[] data)
        {
            _pos = 0;

            if (data != null)
            {
                _buffer = new byte[data.Length];
                _length = (uint)data.Length;
                Buffer.BlockCopy(data, 0, _buffer, 0, data.Length);
            }
        }

        #region PutMethods 放入數據 此方法組會增長緩衝區

        public void PutSbyte(sbyte value)
        {
            WriteBytes(ByteConverter.Int8ToBytes(value));
        }

        public void PutByte(byte value)
        {
            WriteBytes(ByteConverter.UInt8ToBytes(value));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 short 
        /// </summary>
        /// <param name="value">值</param>
        public void PutShortBE(short value)
        {
            WriteBytes(ByteConverter.Int16ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 short 
        /// </summary>
        /// <param name="value">值</param>
        public void PutShortLE(short value)
        {
            WriteBytes(ByteConverter.Int16ToBytesLE(value));
        }

        /// <summary>
        /// 放入 short 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutShort(short value, Endian endian)
        {
            WriteBytes(ByteConverter.Int16ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 ushort 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUshortBE(ushort value)
        {
            WriteBytes(ByteConverter.UInt16ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 ushort 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUshortLE(ushort value)
        {
            WriteBytes(ByteConverter.UInt16ToBytesLE(value));
        }

        /// <summary>
        /// 放入 ushort 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutUshort(ushort value, Endian endian)
        {
            WriteBytes(ByteConverter.UInt16ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 int 
        /// </summary>
        /// <param name="value">值</param>
        public void PutIntBE(int value)
        {
            WriteBytes(ByteConverter.Int32ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 int 
        /// </summary>
        /// <param name="value">值</param>
        public void PutIntLE(int value)
        {
            WriteBytes(ByteConverter.Int32ToBytesLE(value));
        }

        /// <summary>
        /// 放入 int 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutInt(int value, Endian endian)
        {
            WriteBytes(ByteConverter.Int32ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 uint 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUintBE(uint value)
        {
            WriteBytes(ByteConverter.UInt32ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 uint 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUintLE(uint value)
        {
            WriteBytes(ByteConverter.UInt32ToBytesLE(value));
        }

        /// <summary>
        /// 放入 uint 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutUint(uint value, Endian endian)
        {
            WriteBytes(ByteConverter.UInt32ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 long 
        /// </summary>
        /// <param name="value">值</param>
        public void PutLongBE(long value)
        {
            WriteBytes(ByteConverter.Int64ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 long 
        /// </summary>
        /// <param name="value">值</param>
        public void PutLongLE(long value)
        {
            WriteBytes(ByteConverter.Int64ToBytesLE(value));
        }

        /// <summary>
        /// 放入 long 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutLong(long value, Endian endian)
        {
            WriteBytes(ByteConverter.Int64ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 ulong 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUlongBE(ulong value)
        {
            WriteBytes(ByteConverter.UInt64ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 ulong 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUlongLE(ulong value)
        {
            WriteBytes(ByteConverter.UInt64ToBytesLE(value));
        }

        /// <summary>
        /// 放入 ulong 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutUlong(ulong value, Endian endian)
        {
            WriteBytes(ByteConverter.UInt64ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 bool
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">占用长度</param>
        public void PutBoolBE(bool value, byte length)
        {
            WriteBytes(ByteConverter.BoolToBytesBE(value, length));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 bool
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">占用长度</param>
        public void PutBoolLE(bool value, byte length)
        {
            WriteBytes(ByteConverter.BoolToBytesLE(value, length));
        }

        public void PutBool(bool value, byte length, Endian endian)
        {
            WriteBytes(ByteConverter.BoolToBytes(value, length, endian));
        }

        public void PutString(string value, byte prefixLength = 0, byte limitedLength = 0)
        {
            var data = Encoding.UTF8.GetBytes(value);
            PutBytes(data, prefixLength, limitedLength); // 把字符串当作byte[]
        }

        public void PutBytes(byte[] value, byte prefixLength = 0, byte limitedLength = 0)
        {
            bool prefix = prefixLength > 0; // 是否有前缀
            bool limited = limitedLength > 0; // 是否限制长度
            byte[] array; // 处理后的数据
            if (limited) // 限制长度时，写入数据长度=前缀+限制
            {
                limitedLength = (byte)value.Length;
                array = new byte[prefixLength + limitedLength];
                int len = value.Length > limitedLength ? limitedLength : value.Length;
                Buffer.BlockCopy(value, 0, array, prefixLength, len);
            }
            else if (prefix) // 不限制长度且有前缀时，写入数据长度=前缀+value长度
            {
                array = new byte[prefixLength + value.Length];
                Buffer.BlockCopy(value, 0, array, prefixLength, value.Length);
            }
            else // 不限制又没有前缀，写入的就是value本身，不用处理，直接写入
            {
                WriteBytes(value);
                return;
            }
            if (prefix) // 添加前缀，使用大端序
            {
                if (!InsertPrefix(array, (uint)value.Length, prefixLength))
                {
                    throw new IOException("Given prefix length is too small for value bytes."); // 给定的prefixLength不够填充value.Length，终止写入
                }
            }
            WriteBytes(array);
        }

        private uint _pos;
        #endregion

        #region TakeMethods 拿走數據 此方法組會縮短緩衝區
        public long TakeSigned(out long value, uint length, Endian endian = Endian.Big)
        {
            if (CheckAvailable(length))
            {
                value = ByteConverter.BytesToInt(_buffer, _pos, length, endian);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public long TakeSignedBE(out long value, uint length)
        {
            if (CheckAvailable(length))
            {
                value = ByteConverter.BytesToIntBE(_buffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public long TakeSignedLE(out long value, uint length)
        {
            if (CheckAvailable(length))
            {
                value = ByteConverter.BytesToIntLE(_buffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public ulong TakeUnsigned(out ulong value, uint length, Endian endian = Endian.Big)
        {
            if (CheckAvailable(length))
            {
                value = ByteConverter.BytesToUInt(_buffer, _pos, length, endian);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public ulong TakeUnsignedBE(out ulong value, uint length)
        {
            if (CheckAvailable(length))
            {
                value = ByteConverter.BytesToUIntBE(_buffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public ulong TakeUnsignedLE(out ulong value, uint length)
        {
            if (CheckAvailable(length))
            {
                value = ByteConverter.BytesToUIntLE(_buffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public sbyte TakeSbyte(out sbyte value)
        {
            return value = (sbyte)TakeSigned(out long _, 1);
        }

        public byte TakeByte(out byte value)
        {
            return value = (byte)TakeUnsigned(out ulong _, 1);
        }

        public short TakeShortBE(out short value)
        {
            return value = (short)TakeSignedBE(out long _, 2);
        }

        public short TakeShortLE(out short value)
        {
            return value = (short)TakeSignedLE(out long _, 2);
        }

        public short TakeShort(out short value, Endian endian)
        {
            return value = (short)TakeSigned(out long _, 2, endian);
        }

        public ushort TakeUshortBE(out ushort value)
        {
            return value = (ushort)TakeUnsignedBE(out ulong _, 2);
        }

        public ushort TakeUshortLE(out ushort value)
        {
            return value = (ushort)TakeUnsignedLE(out ulong _, 2);
        }

        public ushort TakeUshort(out ushort value, Endian endian)
        {
            return value = (ushort)TakeUnsigned(out ulong _, 2, endian);
        }

        public int TakeIntBE(out int value)
        {
            return value = (int)TakeSignedBE(out long _, 4);
        }

        public int TakeIntLE(out int value)
        {
            return value = (int)TakeSignedLE(out long _, 4);
        }

        public int TakeInt(out int value, Endian endian)
        {
            return value = (int)TakeSigned(out long _, 4, endian);
        }

        public uint TakeUintBE(out uint value)
        {
            return value = (uint)TakeUnsignedBE(out ulong _, 4);
        }

        public uint TakeUintLE(out uint value)
        {
            return value = (uint)TakeUnsignedLE(out ulong _, 4);
        }

        public uint TakeUint(out uint value, Endian endian)
        {
            return value = (uint)TakeUnsigned(out ulong _, 4, endian);
        }

        public long TakeLongBE(out long value)
        {
            return value = TakeSignedBE(out long _, 8);
        }

        public long TakeLongLE(out long value)
        {
            return value = TakeSignedLE(out long _, 8);
        }

        public long TakeLong(out long value, Endian endian)
        {
            return value = TakeSigned(out long _, 8, endian);
        }

        public ulong TakeUlongBE(out ulong value)
        {
            return value = TakeUnsignedBE(out ulong _, 8);
        }

        public ulong TakeUlongLE(out ulong value)
        {
            return value = TakeUnsignedLE(out ulong _, 8);
        }

        public ulong TakeUlong(out ulong value, Endian endian)
        {
            return value = TakeUnsigned(out ulong _, 8, endian);
        }

        public bool TakeBoolBE(out bool value, byte length)
        {
            if (CheckAvailable(length))
            {
                value = ByteConverter.BytesToBoolBE(_buffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public bool TakeBoolLE(out bool value, byte length)
        {
            if (CheckAvailable(length))
            {
                value = ByteConverter.BytesToBoolLE(_buffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public bool TakeBool(out bool value, byte length, Endian endian)
        {
            if (CheckAvailable(length))
            {
                value = ByteConverter.BytesToBool(_buffer, _pos, length, endian);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public string TakeString(out string value, Prefix prefixFlag)
        {
            return value = Encoding.UTF8.GetString(TakeBytes(out byte[] _, prefixFlag));
        }

        public byte[] TakeBytes(out byte[] value, Prefix prefixFlag)
        {
            uint length;
            bool reduce = (prefixFlag & Prefix.WithPrefix) > 0;
            uint preLen = ((uint)prefixFlag) & 15;
            switch (preLen)
            {
                case 0: // Read to end.
                    length = _length - _pos;
                    break;
                case 1:
                case 2:
                case 4:
                    if (CheckAvailable(preLen))
                    {
                        length = (uint)ByteConverter.BytesToUIntBE(_buffer, _pos, preLen);
                        _pos += preLen;
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
                    throw _bufferErr;
                default:
                    throw new ArgumentOutOfRangeException("Invalid prefix flag.");
            }
            if (CheckAvailable(length))
            {
                value = new byte[length];
                Buffer.BlockCopy(_buffer, (int)_pos, value, 0, (int)length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public byte[] TakeBytes(out byte[] value, uint length)
        {
            if (CheckAvailable(length))
            {
                value = new byte[length];
                Buffer.BlockCopy(_buffer, (int)_pos, value, 0, (int)length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public byte[] TakeAllBytes(out byte[] value)
        {
            value = new byte[_length - _pos];
            Buffer.BlockCopy(_buffer, (int)_pos, value, 0, value.Length);
            _pos = _length;
            return value;
        }

        /// <summary>
        /// 吃掉數據 φ(゜▽゜*)♪
        /// </summary>
        /// <param name="length"></param>
        public void EatBytes(uint length)
        {
            _pos += length;
        }

        #endregion

        #region GetMethods 獲取數據 此方法組不會對緩衝區造成影響

        /// <summary>
        /// 獲取打包數據
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            if (_length > 0)
            {
                var data = new byte[_length];
                Buffer.BlockCopy(_buffer, 0, data, 0, (int)_length);
                return data;
            }
            return new byte[0];
        }

        #endregion

        public uint Length
        {
            get { return _length; }
        }

        public uint RemainLength
        {
            get { return _length - _pos; }
        }

        /// <summary>
        /// 寫入數據
        /// </summary>
        /// <param name="data">數據</param>
        public void WriteBytes(byte[] data)
        {
            // 分配新的空間
            uint targetLength = _length + (uint)data.Length;
            uint len = targetLength >> 10;
            if ((targetLength & 1023) > 0)
            {
                ++len;
            }
            len <<= 10;
            if (_buffer == null)
            {
                _buffer = new byte[len];
            }
            else if (_buffer.Length < targetLength)
            {
                Array.Resize(ref _buffer, (int)len);
            }

            // 寫入數據
            Buffer.BlockCopy(data, 0, _buffer, (int)_length, data.Length);

            // 更新長度
            _length = targetLength;
        }

        public bool CheckAvailable(uint length = 0)
        {
            return _pos + length <= _length;
        }

        public static bool InsertPrefix(byte[] array, uint value, uint size, uint offset = 0, Endian endian = Endian.Big)
        {
            uint minLen = 0; // 表示value需要最少多少字节
            uint valLen = value; // 计算value各字节数值的临时变量
            while (valLen > 0)
            {
                ++minLen;
                valLen >>= 8;
            }
            if (minLen > size)
            {
                return false; // value数据太长，前缀无法表示其长度
            }
            valLen = value;
            if (endian == Endian.Big)
            {
                for (int i = 0, j = (int)size - 1; i < minLen; ++i, --j)
                {
                    array[j + offset] = (byte)(valLen & 255); // 每次取最低一字节从后往前写入
                    valLen >>= 8;
                }
            }
            else
            {
                for (int i = 0; i < minLen; ++i)
                {
                    array[i + offset] = (byte)(valLen & 255); // 每次取最低一字节从前往后写入
                    valLen >>= 8;
                }
            }
            return true;
        }

        #region Operators

        public static ByteBuffer operator +(ByteBuffer a, ByteBuffer b)
        {
            var packet = new ByteBuffer();
            packet.PutBytes(a.GetBytes());
            packet.PutBytes(b.GetBytes());
            return packet;
        }

        public static ByteBuffer operator +(byte[] a, ByteBuffer b)
        {
            var packet = new ByteBuffer();
            packet.PutBytes(a);
            packet.PutBytes(b.GetBytes());
            return packet;
        }

        public static ByteBuffer operator +(ByteBuffer a, byte[] b)
        {
            var packet = new ByteBuffer();
            packet.PutBytes(a.GetBytes());
            packet.PutBytes(b);
            return packet;
        }

        #endregion
    }
}
