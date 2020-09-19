using System;
using System.Text;
using Konata.Utils;
using Konata.Msf.Utils.Crypt;
using System.IO;

namespace Konata.Msf
{
    public class Packet
    {
        public enum Endian
        {
            Big,
            Little
        }

        public enum Prefix
        {
            None = 0, // 前綴類型
            Uint8 = 1,
            Uint16 = 2,
            Uint32 = 4,

            LengthOnly = 64, // 只包括數據長度
            WithPrefix = 128,  // 包括數據長度和前綴長度
        }

        public enum ReadWrite
        {
            Write,
            Read,
            //ReadAndWrite
        }

        private ReadWrite _flag;
        private byte[] _packetBuffer;
        private int _packetLength;
        private static readonly IOException _bufferErr = 
            new IOException("Insufficient buffer space.");

        public Packet()
        {
            _flag = ReadWrite.Write;
        }

        public Packet(byte[] data)
        {
            _pos = 0;
            _flag = ReadWrite.Read;

            _packetBuffer = new byte[data.Length];
            Buffer.BlockCopy(data, 0, _packetBuffer, 0, data.Length);
        }

        public Packet(byte[] data, ICryptor cryptor, byte[] cryptKey)
        {
            _flag = ReadWrite.Read;
            //
        }

        #region PutMethods 放入數據 此方法組會增長緩衝區

        public void PutSbyte(sbyte value)
        {
            WriteBytes(BufferIO.Int8ToBytes(value));
        }

        public void PutByte(byte value)
        {
            WriteBytes(BufferIO.UInt8ToBytes(value));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 short 
        /// </summary>
        /// <param name="value">值</param>
        public void PutShortBE(short value)
        {
            WriteBytes(BufferIO.Int16ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 short 
        /// </summary>
        /// <param name="value">值</param>
        public void PutShortLE(short value)
        {
            WriteBytes(BufferIO.Int16ToBytesLE(value));
        }

        /// <summary>
        /// 放入 short 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutShort(short value, Endian endian)
        {
            WriteBytes(BufferIO.Int16ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 ushort 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUshortBE(ushort value)
        {
            WriteBytes(BufferIO.UInt16ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 ushort 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUshortLE(ushort value)
        {
            WriteBytes(BufferIO.UInt16ToBytesLE(value));
        }

        /// <summary>
        /// 放入 ushort 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutUshort(ushort value, Endian endian)
        {
            WriteBytes(BufferIO.UInt16ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 int 
        /// </summary>
        /// <param name="value">值</param>
        public void PutIntBE(int value)
        {
            WriteBytes(BufferIO.Int32ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 int 
        /// </summary>
        /// <param name="value">值</param>
        public void PutIntLE(int value)
        {
            WriteBytes(BufferIO.Int32ToBytesLE(value));
        }

        /// <summary>
        /// 放入 int 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutInt(int value, Endian endian)
        {
            WriteBytes(BufferIO.Int32ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 uint 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUintBE(uint value)
        {
            WriteBytes(BufferIO.UInt32ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 uint 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUintLE(uint value)
        {
            WriteBytes(BufferIO.UInt32ToBytesLE(value));
        }

        /// <summary>
        /// 放入 uint 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutUint(uint value, Endian endian)
        {
            WriteBytes(BufferIO.UInt32ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 long 
        /// </summary>
        /// <param name="value">值</param>
        public void PutLongBE(long value)
        {
            WriteBytes(BufferIO.Int64ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 long 
        /// </summary>
        /// <param name="value">值</param>
        public void PutLongLE(long value)
        {
            WriteBytes(BufferIO.Int64ToBytesLE(value));
        }

        /// <summary>
        /// 放入 long 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutLong(long value, Endian endian)
        {
            WriteBytes(BufferIO.Int64ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 ulong 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUlongBE(ulong value)
        {
            WriteBytes(BufferIO.UInt64ToBytesBE(value));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 ulong 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUlongLE(ulong value)
        {
            WriteBytes(BufferIO.UInt64ToBytesLE(value));
        }

        /// <summary>
        /// 放入 ulong 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutUlong(ulong value, Endian endian)
        {
            WriteBytes(BufferIO.UInt64ToBytes(value, endian));
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 bool
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">占用长度</param>
        public void PutBoolBE(bool value, byte length)
        {
            WriteBytes(BufferIO.BoolToBytesBE(value, length));
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 bool
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">占用长度</param>
        public void PutBoolLE(bool value, byte length)
        {
            WriteBytes(BufferIO.BoolToBytesLE(value, length));
        }

        public void PutBool(bool value, byte length, Endian endian)
        {
            WriteBytes(BufferIO.BoolToBytes(value, length, endian));
        }

        public void PutString(string value, byte prefixLength = 0, byte limitedLength = 0)
        {
            var data = Encoding.UTF8.GetBytes(value);
            PutBytes(data, prefixLength, limitedLength); // 把字符串当作byte[]
        }

        public void PutHexString(string value, byte prefixLength = 0, byte limitedLength = 0)
        {
            var data = Hex.HexStr2Bytes(value);
            PutBytes(data, prefixLength, limitedLength);
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
                if (!InsertPrefix(array, value.Length, prefixLength))
                {
                    throw new IOException("Given prefix length is too small for value bytes."); // 给定的prefixLength不够填充value.Length，终止写入
                }
            }
            WriteBytes(array);
        }

        public void PutEncryptedBytes(byte[] value, ICryptor cryptor, byte[] cryptKey)
        {
            WriteBytes(cryptor.Encrypt(value, cryptKey));
        }

        public void PutEncryptedBytes(byte[] value, ICryptor cryptor, byte[] cryptKey,
            byte prefixLength = 0, byte limitedLength = 0)
        {
            PutBytes(cryptor.Encrypt(value, cryptKey), prefixLength, limitedLength);
        }

        /// <summary>
        /// 放入 Packet
        /// </summary>
        /// <param name="value"></param>
        public void PutPacket(Packet value)
        {
            PutBytes(value.GetBytes());
        }

        /// <summary>
        /// 加密 Packet 放入
        /// </summary>
        /// <param name="value"></param>
        public void PutEncryptedPacket(Packet value, ICryptor cryptor, byte[] cryptKey)
        {
            PutEncryptedBytes(value.GetBytes(), cryptor, cryptKey);
        }

        /// <summary>
        /// 放入 Tlv
        /// </summary>
        /// <param name="value"></param>
        public void PutTlv(Packet value)
        {
            PutBytes(value.GetBytes());
        }

        private uint _pos;
        #endregion

        #region TakeMethods 拿走數據 此方法組會縮短緩衝區
        public long TakeSigned(out long value, uint length, Endian endian = Endian.Big)
        {
            if (CheckAvailable(length))
            {
                value = BufferIO.BytesToInt(_packetBuffer, _pos, length, endian);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public long TakeSignedBE(out long value, uint length)
        {
            if (CheckAvailable(length))
            {
                value = BufferIO.BytesToIntBE(_packetBuffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public long TakeSignedLE(out long value, uint length)
        {
            if (CheckAvailable(length))
            {
                value = BufferIO.BytesToIntLE(_packetBuffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public ulong TakeUnsigned(out ulong value, uint length, Endian endian = Endian.Big)
        {
            if (CheckAvailable(length))
            {
                value = BufferIO.BytesToUInt(_packetBuffer, _pos, length, endian);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public ulong TakeUnsignedBE(out ulong value, uint length)
        {
            if (CheckAvailable(length))
            {
                value = BufferIO.BytesToUIntBE(_packetBuffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public ulong TakeUnsignedLE(out ulong value, uint length)
        {
            if (CheckAvailable(length))
            {
                value = BufferIO.BytesToUIntLE(_packetBuffer, _pos, length);
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
                value = BufferIO.BytesToBoolBE(_packetBuffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public bool TakeBoolLE(out bool value, byte length)
        {
            if (CheckAvailable(length))
            {
                value = BufferIO.BytesToBoolLE(_packetBuffer, _pos, length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public bool TakeBool(out bool value, byte length, Endian endian)
        {
            if (CheckAvailable(length))
            {
                value = BufferIO.BytesToBool(_packetBuffer, _pos, length, endian);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public string TakeString(out string value, Prefix prefixFlag)
        {
            return value = Encoding.UTF8.GetString(TakeBytes(out byte[] _, prefixFlag));
        }

        public string TakeHexString(out string value, Prefix prefixFlag)
        {
            return value = Hex.Bytes2HexStr(TakeBytes(out byte[] _, prefixFlag));
        }

        public byte[] TakeBytes(out byte[] value, Prefix prefixFlag)
        {
            uint length;
            bool reduce = (prefixFlag & Prefix.WithPrefix) > 0;
            uint preLen = ((uint)prefixFlag) & 15;
            switch (preLen)
            {
                case 0: // Read to end.
                    length = (uint)(_packetLength - _pos);
                    break;
                case 1:
                case 2:
                case 4:
                    if (CheckAvailable(preLen))
                    {
                        length = (uint)BufferIO.BytesToUIntBE(_packetBuffer, _pos, preLen);
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
                Buffer.BlockCopy(_packetBuffer, (int)_pos, value, 0, (int)length);
                _pos += length;
                return value;
            }
            throw _bufferErr;
        }

        public byte[] TakeDecryptedBytes(out byte[] value, ICryptor cryptor, byte[] cryptKey,
            Prefix prefixFlag = Prefix.None)
        {
            return value = cryptor.Decrypt(TakeBytes(out var _, prefixFlag), cryptKey);
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
            var data = new byte[_packetLength];
            Buffer.BlockCopy(_packetBuffer, 0, data, 0, _packetLength);

            return data;
        }

        /// <summary>
        /// 獲取打包數據並加密
        /// </summary>
        /// <returns></returns>
        public byte[] GetEncryptedBytes(ICryptor cryptor, byte[] cryptKey)
        {
            return cryptor.Encrypt(GetBytes(), cryptKey);
        }

        /// <summary>
        /// 到字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Hex.Bytes2HexStr(GetBytes());
        }

        #endregion

        #region BarrierMethods 屏障方法會計算PutMethods放入數據的長度 並寫到它們的前面

        private bool _barEnc = false;
        private byte[] _barEncBuffer;
        private uint _barEncLength;
        private ICryptor _barEncCryptor;
        private byte[] _barEncKey;

        /// <summary>
        /// [進入屏障] 在這之後透過 PutMethods 方法組放入的數據將被計算長度
        /// </summary>
        /// <param name="lengthSize"></param>
        /// <param name="endian"></param>
        protected void EnterBarrier(int lengthSize, Endian endian)
        {
            _barPos = _packetLength;
            _lenSize = lengthSize;
            _barLenEndian = endian;
            PutBytes(new byte[lengthSize]);
        }

        protected void EnterBarrierEncrypted(int lengthSize, Endian endian, ICryptor cryptor, byte[] cryptKey)
        {
            EnterBarrier(lengthSize, endian);
            _barEnc = true;
            _barEncBuffer = _packetBuffer;
            _barEncLength = _packetLength;
            _barEncCryptor = cryptor;
            _barEncKey = cryptKey;
            _packetBuffer = null;
            _packetLength = 0;
        }

        /// <summary>
        /// [離開屏障] 會立即在加入的數據前寫入長度
        /// </summary>
        protected void LeaveBarrier()
        {
            if (_barEnc)
            {
                byte[] enc = GetEncryptedBytes(_barEncCryptor, _barEncKey);
                _packetBuffer = _barEncBuffer;
                _packetLength = _barEncLength;
                PutBytes(enc);
                _barEnc = false;
                _barEncBuffer = null;
                _barEncLength = 0;
                _barEncCryptor = null;
                _barEncKey = null;
            }
            InsertPrefix(_packetBuffer, _packetLength - _barPos - _lenSize,
                _lenSize, _barPos, _barLenEndian);
        }

        #endregion

        public int Length
        {
            get { return _packetLength; }
        }

        /// <summary>
        /// 寫入數據
        /// </summary>
        /// <param name="data">數據</param>
        private void WriteBytes(byte[] data)
        {
            // 分配新的空間
            var targetLength = _packetLength + data.Length;
            int len = targetLength >> 10;
            if ((targetLength & 1023) > 0)
            {
                ++len;
            }
            len <<= 10;
            if (_packetBuffer == null)
            {
                _packetBuffer = new byte[len];
            }
            else if (_packetBuffer.Length < targetLength)
            {
                Array.Resize(ref _packetBuffer, len);
            }

            // 寫入數據
            Buffer.BlockCopy(data, 0, _packetBuffer, _packetLength, data.Length);

            // 更新長度
            _packetLength = targetLength;
        }

        private bool CheckAvailable(uint length = 0)
        {
            return _pos + length <= _packetLength;
        }

        private int _barPos;
        private int _lenSize;
        private Endian _barLenEndian;

        private static bool InsertPrefix(byte[] array, int value, int size, int offset = 0, Endian endian = Endian.Big)
        {
            int minLen = 0; // 表示value需要最少多少字节
            int valLen = value; // 计算value各字节数值的临时变量
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
                for (int i = 0, j = size - 1; i < minLen; ++i, --j)
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
        public static Packet operator +(Packet a, Packet b)
        {
            var packet = new Packet();
            packet.PutPacket(a);
            packet.PutPacket(b);
            return packet;
        }

        public static Packet operator +(byte[] a, Packet b)
        {
            var packet = new Packet();
            packet.PutBytes(a);
            packet.PutPacket(b);
            return packet;
        }

        public static Packet operator +(Packet a, byte[] b)
        {
            var packet = new Packet();
            packet.PutPacket(a);
            packet.PutBytes(b);
            return packet;
        }
        #endregion
    }
}
