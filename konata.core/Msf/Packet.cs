using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        private static int ResizeArray(ref byte[] data, int minLen)
        {
            int len = minLen >> 10;
            if ((minLen & 1023) > 0)
            {
                ++len;
            }
            len <<= 10;
            if (data == null)
            {
                data = new byte[len];
            }
            else if (data.Length < minLen)
            {
                Array.Resize(ref data, len);
            }
            return data.Length;
        }

        /// <summary>
        /// 寫入數據
        /// </summary>
        /// <param name="data">數據</param>
        /// <param name="endian">字節序</param>
        private void WriteBytes(byte[] data, Endian endian)
        {
            // 分配新的空間

            var targetLength = _packetLength + data.Length;
            ResizeArray(ref _packetBuffer, targetLength);

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
            WriteBytes(new byte[] { (byte)value }, Endian.Little);
        }

        public void PutByte(byte value)
        {
            WriteBytes(new byte[] { value }, Endian.Little);
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
            WriteBytes(new byte[] { (byte)(value >> 8), (byte)(value & 255) }, endian);
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
            WriteBytes(new byte[] { (byte)(value >> 8), (byte)(value & 255) }, endian);
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
                (byte)(value & 255) }, endian);
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
                (byte)(value & 255) }, endian);
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
                (byte)(value & 255) }, endian);
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
                (byte)(value & 255) }, endian);
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
            byte[] array = new byte[length];
            if (value)
            {
                array[endian == Endian.Little ? length - 1 : 0] = true;
            }
            WriteBytes(array, endian);
        }

        public void PutString(string value, byte prefixLength = 0, byte limitedLength = 0)
        {
            var str = Encoding.UTF8.GetBytes(value);

        }

        public void PutBytes(byte[] value, byte prefixLength = 0, byte limitedLength = 0)
        {

        }

        public void PutEncryptBytes(byte[] value)
        {

        }



        public void PutPacket(Packet value)
        {
            PutBytes(value.GetBytes());
        }

        public void PutTlv(Packet value)
        {
            PutBytes(value.GetBytes());
        }

        public void FromPacket(Packet packet)
        {

        }

        public byte[] GetBytes()
        {
            var data = new byte[_packetLength];
            Array.Copy(_packetBuffer, data, _packetLength);

            _packetBuffer = new byte[0];
            _packetLength = 0;

            return data;
        }

        public static Packet operator +(Packet a, Packet b)
        {
            var packet = new Packet();
            packet.PutPacket(a);
            packet.PutPacket(b);
            return packet;
        }

    }
}