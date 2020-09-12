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

        private byte[] _packetBuffer;
        private int _packetLength;

        /// <summary>
        /// 重新分配緩衝區
        /// </summary>
        /// <param name="data">緩衝區</param>
        /// <param name="minLen">最小長度</param>
        /// <returns></returns>
        //private static int ResizeArray(ref byte[] _packetBuffer, int targetLength)
        //{
        //    int len = targetLength >> 10;
        //    if ((targetLength & 1023) > 0)
        //    {
        //        ++len;
        //    }
        //    len <<= 10;
        //    if (_packetBuffer == null)
        //    {
        //        _packetBuffer = new byte[len];
        //    }
        //    else if (_packetBuffer.Length < targetLength)
        //    {
        //        Array.Resize(ref _packetBuffer, len);
        //    }
        //}

        /// <summary>
        /// 寫入數據
        /// </summary>
        /// <param name="data">數據</param>
        /// <param name="endian">字節序</param>
        private void WriteBytes(byte[] data, uint length, Endian endian = Endian.Big)
        {
            // 分配新的空間
            var targetLength = _packetLength + data.Length;
            //ResizeArray(ref _packetBuffer, targetLength);
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

            // 寫入方向
            var step = endian == Endian.Little ? -1 : 1;
            var position = step == -1 ? targetLength - 1 : _packetLength;

            // 寫入數據
            for (int i = 0; i < data.Length; ++i, position += step)
            {
                _packetBuffer[position] = data[i];
            }

            // 更新長度
            _packetLength = targetLength;
        }

        public void PutSbyte(sbyte value)
        {
            WriteBytes(new byte[] { (byte)value }, 1);
        }

        public void PutByte(byte value)
        {
            WriteBytes(new byte[] { value }, 1);
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
                2, endian);
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
                2, endian);
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
                4, endian);
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
                4, endian);
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
                8, endian);
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
                8, endian);
        }

        /// <summary>
        /// 以 Big Endian 字节序, 放入 bool
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">占用长度</param>
        public void PutBoolBE(bool value, byte length)
        {
            PutBoolBE(value, length, Endian.Big);
        }

        /// <summary>
        /// 以 Little Endian 字节序, 放入 bool
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">占用长度</param>
        public void PutBoolBELE(bool value, byte length)
        {
            PutBoolBE(value, length, Endian.Little);
        }

        public void PutBoolBE(bool value, byte length, Endian endian)
        {
            var data = new byte[length];
            if (value)
            {
                data[endian == Endian.Little ? length - 1 : 0] = 1;
            }
            WriteBytes(data, length, endian);
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
                WriteBytes(value, (uint)value.Length);
                return;
            }
            if (prefix) // 添加前缀
            {
                int minLen = 0; // 表示value的长度需要最少多少字节
                int valLen = value.Length; // value长度的临时变量
                while (valLen > 0)
                {
                    ++minLen;
                    valLen >>= 8;
                }
                if (minLen > prefixLength)
                {
                    return; // value数据太长，前缀无法表示其长度
                }
                valLen = value.Length;
                for (int i = 0, j = prefixLength - 1; i < minLen; ++i, --j)
                {
                    array[j] = (byte)(valLen & 255); // 每次取最低一字节从后往前写入，这里规定前缀是大端序
                    valLen >>= 8;
                }
            }
            WriteBytes(array, (uint)array.Length);
        }

        public void PutEncryptBytes(byte[] value, ICryptor cryptor, byte[] key)
        {
            byte[] data = cryptor.Encrypt(value, key);
            WriteBytes(data, (uint)data.Length);
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
        /// 放入 Tlv
        /// </summary>
        /// <param name="value"></param>
        public void PutTlv(Packet value)
        {
            PutBytes(value.GetBytes());
        }

        /// <summary>
        /// 從 Packet 讀取
        /// </summary>
        /// <param name="packet"></param>
        public void FromPacket(Packet packet)
        {
            _packetBuffer = packet.GetBytes();
            _packetLength = _packetBuffer.Length;
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

        //public override string ToString()
        //{
        //    var data = new byte[_packetLength];
        //    Buffer.BlockCopy(_packetBuffer, 0, data, 0, _packetLength);

        //    return Hex.Bytes2HexStr(data);
        //}

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
    }
}