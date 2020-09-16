using System;
using System.Text;
using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf
{

    public class Packet
    {

        public enum Endian
        {
            Big,
            Little
        }

        public enum RWFlag
        {
            Read,
            Write,
            //ReadAndWrite
        }

        private byte[] _packetBuffer;
        private int _packetLength;

        private RWFlag _flag;

        public Packet()
        {
            _flag = RWFlag.Write;
        }

        public Packet(byte[] data)
        {
            _flag = RWFlag.Read;

            _packetBuffer = new byte[data.Length];
            Buffer.BlockCopy(data, 0, _packetBuffer, 0, data.Length);
        }

        public Packet(byte[] data, ICryptor cryptor, byte[] cryptKey)
        {

        }

        /// <summary>
        /// 寫入數據
        /// </summary>
        /// <param name="data">數據</param>
        /// <param name="endian">字節序</param>
        private void WriteBytes(byte[] data, Endian endian = Endian.Big)
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

            //// 寫入方向
            //var step = endian == Endian.Little ? -1 : 1;
            //var position = step == -1 ? targetLength - 1 : _packetLength;

            //// 寫入數據
            //for (int i = 0; i < data.Length; ++i, position += step)
            //{
            //    _packetBuffer[position] = data[i];
            //}

            // 寫入數據
            if (endian == Endian.Big)
            {
                Buffer.BlockCopy(data, 0, _packetBuffer, _packetLength, data.Length);
            }
            else
            {
                for (int i = 0, j = targetLength - 1; i < data.Length; ++i, --j)
                {
                    _packetBuffer[j] = data[i];
                }
            }

            // 更新長度
            _packetLength = targetLength;
        }

        public void PutSbyte(sbyte value)
        {
            WriteBytes(new byte[] { (byte)value });
        }

        public void PutByte(byte value)
        {
            WriteBytes(new byte[] { value });
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 ushort 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUshortBE(ushort value)
        {
            PutUshort(value, Endian.Big);
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 ushort 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUshortLE(ushort value)
        {
            PutUshort(value, Endian.Little);
        }

        /// <summary>
        /// 放入 ushort 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutUshort(ushort value, Endian endian)
        {
            WriteBytes(new byte[] {
                (byte)(value >> 8),
                (byte)(value & 255) },
                endian);
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 short 
        /// </summary>
        /// <param name="value">值</param>
        public void PutShortBE(short value)
        {
            PutShort(value, Endian.Big);
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 short 
        /// </summary>
        /// <param name="value">值</param>
        public void PutShortLE(short value)
        {
            PutShort(value, Endian.Little);
        }

        /// <summary>
        /// 放入 short 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutShort(short value, Endian endian)
        {
            WriteBytes(new byte[] {
                (byte)(value >> 8),
                (byte)(value & 255) },
                endian);
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 int 
        /// </summary>
        /// <param name="value">值</param>
        public void PutIntBE(int value)
        {
            PutInt(value, Endian.Big);
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 int 
        /// </summary>
        /// <param name="value">值</param>
        public void PutIntLE(int value)
        {
            PutInt(value, Endian.Little);
        }

        /// <summary>
        /// 放入 int 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutInt(int value, Endian endian)
        {
            WriteBytes(new byte[] {
                (byte)(value >> 24),
                (byte)((value >> 16) & 255),
                (byte)((value >> 8) & 255),
                (byte)(value & 255) },
                endian);
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 uint 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUintBE(uint value)
        {
            PutUint(value, Endian.Big);
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 uint 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUintLE(uint value)
        {
            PutUint(value, Endian.Little);
        }

        /// <summary>
        /// 放入 uint 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutUint(uint value, Endian endian)
        {
            WriteBytes(new byte[] {
                (byte)(value >> 24),
                (byte)((value >> 16) & 255),
                (byte)((value >> 8) & 255),
                (byte)(value & 255) },
                endian);
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 long 
        /// </summary>
        /// <param name="value">值</param>
        public void PutLongBE(long value)
        {
            PutLong(value, Endian.Big);
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 long 
        /// </summary>
        /// <param name="value">值</param>
        public void PutLongLE(long value)
        {
            PutLong(value, Endian.Little);
        }

        /// <summary>
        /// 放入 long 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutLong(long value, Endian endian)
        {
            WriteBytes(new byte[] {
                (byte)(value >> 56),
                (byte)((value >> 48) & 255),
                (byte)((value >> 40) & 255),
                (byte)((value >> 32) & 255),
                (byte)((value >> 24) & 255),
                (byte)((value >> 16) & 255),
                (byte)((value >> 8) & 255),
                (byte)(value & 255) },
                endian);
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 ulong 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUlongBE(ulong value)
        {
            PutUlong(value, Endian.Big);
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 ulong 
        /// </summary>
        /// <param name="value">值</param>
        public void PutUlongLE(ulong value)
        {
            PutUlong(value, Endian.Little);
        }

        /// <summary>
        /// 放入 ulong 
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="endian">字节序</param>
        public void PutUlong(ulong value, Endian endian)
        {
            WriteBytes(new byte[] {
                (byte)(value >> 56),
                (byte)((value >> 48) & 255),
                (byte)((value >> 40) & 255),
                (byte)((value >> 32) & 255),
                (byte)((value >> 24) & 255),
                (byte)((value >> 16) & 255),
                (byte)((value >> 8) & 255),
                (byte)(value & 255) },
                endian);
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 bool
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">占用长度</param>
        public void PutBoolBE(bool value, byte length)
        {
            PutBool(value, length, Endian.Big);
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 bool
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">占用长度</param>
        public void PutBoolLE(bool value, byte length)
        {
            PutBool(value, length, Endian.Little);
        }

        public void PutBool(bool value, byte length, Endian endian)
        {
            var data = new byte[length];
            if (value)
            {
                data[endian == Endian.Little ? length - 1 : 0] = 1;
            }
            WriteBytes(data, endian);
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
                if (!InsertLength(array, value.Length, prefixLength))
                {
                    return; // 给定的prefixLength不够填充value.Length，终止写入
                }
            }
            WriteBytes(array);
        }

        public void PutEncryptedBytes(byte[] value, ICryptor cryptor, byte[] cryptKey)
        {
            byte[] data = cryptor.Encrypt(value, cryptKey);
            WriteBytes(data);
        }

        public void PutEncryptedBytes(byte[] value, ICryptor cryptor, byte[] cryptKey,
            byte prefixLength = 0, byte limitedLength = 0)
        {
            byte[] data = cryptor.Encrypt(value, cryptKey);
            PutBytes(data, prefixLength, limitedLength);
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

        public sbyte TakeSbyte(out sbyte value)
        {
            throw new NotImplementedException();
        }

        public byte TakeByte(out byte value)
        {
            throw new NotImplementedException();
        }

        public ushort TakeUshortBE(out ushort value)
        {
            throw new NotImplementedException();
        }

        public ushort TakeUshortLE(out ushort value)
        {
            throw new NotImplementedException();
        }

        public ushort TakeUshort(out ushort value, Endian endian)
        {
            throw new NotImplementedException();
        }

        public short TakeShortBE(out short value)
        {
            throw new NotImplementedException();
        }

        public short TakeShortLE(out short value)
        {
            throw new NotImplementedException();
        }

        public short TakeShort(out short value, Endian endian)
        {
            throw new NotImplementedException();
        }

        public int TakeIntBE(out int value)
        {
            throw new NotImplementedException();
        }

        public int TakeIntLE(out int value)
        {
            throw new NotImplementedException();
        }

        public int TakeInt(out int value, Endian endian)
        {
            throw new NotImplementedException();
        }

        public uint TakeUintBE(out uint value)
        {
            throw new NotImplementedException();
        }

        public uint TakeUintLE(out uint value)
        {
            throw new NotImplementedException();
        }

        public uint TakeUint(out uint value, Endian endian)
        {
            throw new NotImplementedException();
        }

        public long TakeLongBE(out long value)
        {
            throw new NotImplementedException();
        }

        public long TakeLongLE(out long value)
        {
            throw new NotImplementedException();
        }

        public long TakeLong(out long value, Endian endian)
        {
            throw new NotImplementedException();
        }

        public ulong TakeUlongBE(out ulong value)
        {
            throw new NotImplementedException();
        }

        public ulong TakeUlongLE(out ulong value)
        {
            throw new NotImplementedException();
        }

        public ulong TakeUlong(out ulong value, Endian endian)
        {
            throw new NotImplementedException();
        }

        public bool TakeBoolBE(out bool value, byte length)
        {
            throw new NotImplementedException();
        }

        public bool TakeBoolLE(out bool value, byte length)
        {
            throw new NotImplementedException();
        }

        public bool TakeBool(out bool value, byte length, Endian endian)
        {
            throw new NotImplementedException();
        }

        public string TakeString(out string value, uint length, byte prefixLength = 0)
        {
            throw new NotImplementedException();
        }

        public string TakeHexString(out string value, uint length, byte prefixLength = 0)
        {
            throw new NotImplementedException();
        }

        public byte[] TakeBytes(out byte[] value, uint length, byte prefixLength = 0)
        {
            throw new NotImplementedException();
        }

        public byte[] TakeDecryptedBytes(out byte[] value, ICryptor cryptor, byte[] cryptKey)
        {
            throw new NotImplementedException();
        }

        public byte[] TakeDecryptedBytes(out byte[] value, ICryptor cryptor, byte[] cryptKey,
            byte prefixLength = 0, byte limitedLength = 0)
        {
            throw new NotImplementedException();
        }

        //public Packet TakePacket(out Packet value)
        //{
        //    throw new NotImplementedException();
        //}

        //public Packet TakeDecryptedPacket(out Packet value, ICryptor cryptor, byte[] cryptKey)
        //{
        //    throw new NotImplementedException();
        //}

        //public Packet TakeTlv(out Packet value)
        //{
        //    throw new NotImplementedException();
        //}

        public void EatBytes(int count)
        {

        }

        /// <summary>
        /// 獲取打包數據
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetBytes()
        {
            var data = new byte[_packetLength];
            Buffer.BlockCopy(_packetBuffer, 0, data, 0, _packetLength);

            _packetBuffer = null;
            _packetLength = 0;

            return data;
        }

        public string ToHexString()
        {
            return Hex.Bytes2HexStr(GetBytes());
        }

        /// <summary>
        /// 獲取打包數據並加密
        /// </summary>
        /// <returns></returns>
        public byte[] GetEncryptedBytes(ICryptor cryptor, byte[] cryptKey)
        {
            return cryptor.Encrypt(GetBytes(), cryptKey);
        }

        public int Length
        {
            get { return _packetLength; }
        }

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

        protected void EnterBarrier(int lengthSize, Endian endian)
        {
            _barPos = _packetLength;
            _lenSize = lengthSize;
            _barLenEndian = endian;
            PutBytes(new byte[lengthSize]);
        }

        protected void LeaveBarrier()
        {
            InsertLength(_packetBuffer, _packetLength - _barPos - _lenSize, _lenSize, _barPos, _barLenEndian);
        }

        private int _barPos;
        private int _lenSize;
        private Endian _barLenEndian;

        private static bool InsertLength(byte[] array, int value, int size, int offset = 0, Endian endian = Endian.Big)
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
    }
}