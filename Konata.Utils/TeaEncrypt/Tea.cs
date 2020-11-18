using System;

namespace Konata.Utils.TeaEncrypt
{
    public class Tea
    {
        /// <summary>
        /// Encrypt data.
        /// </summary>
        public byte[] Encrypt(byte[] data, byte[] key)
        {
            if (data == null || key == null || key.Length < 16)
            {
                return null;
            }
            int inputLength = data.Length;
            int fill = ((8 - ((inputLength + 10) & 7)) & 7) + 2;
            int length = fill + inputLength + 8;
            // Thus length must be divisible by 8.
            //if ((length & 7) != 0 || (length >> 4) == 0)
            //{
            //    return null; // Invalid plain data length.
            //}
            byte[] plain = new byte[length];
            byte[] cipher = new byte[length];
            byte[] plainXorPrev = new byte[8];
            byte[] tempCipher = new byte[8];
            // First byte represents to length of fill bytes - 2.
            plain[0] = (byte)(RandomByte(248) | (fill - 2));
            for (int i = 1; i <= fill; ++i)
            {
                plain[i] = RandomByte();
            }
            Buffer.BlockCopy(data, 0, plain, fill + 1, inputLength);
            // Encrypt data.
            for (int i = 0; i < length; i += 8)
            {
                byte[] plainXor = Xor8(plain, tempCipher, i);
                tempCipher = Xor8(EnCipher(plainXor, key), plainXorPrev);
                plainXorPrev = plainXor;
                Buffer.BlockCopy(tempCipher, 0, cipher, i, 8);
            }
            return cipher;
        }

        /// <summary>
        /// Decrypt data.
        /// </summary>
        public byte[] Decrypt(byte[] data, byte[] key)
        {
            if (data == null || key == null || key.Length < 16)
            {
                return null;
            }
            int length = data.Length;
            if ((length & 7) != 0 || (length >> 4) == 0)
            {
                return null; // Invalid cipher data length.
            }
            byte[] plain = new byte[length];
            byte[] plainSub = new byte[8];
            // Decrypt data.
            for (int i = 0; i < length; i += 8)
            {
                plainSub = DeCipher(Xor8(data, plainSub, i), key);
                Buffer.BlockCopy(Xor8(plainSub, data, 0, i - 8), 0, plain, i, 8);
            }
            // Verify that the last 7 bytes are 0.
            for (int i = length - 7; i < length; ++i)
            {
                if (plain[i] != 0)
                {
                    return null; // Verification failed.
                }
            }
            // Extract valid data.
            int from = (plain[0] & 7) + 3;
            byte[] output = new byte[length - from - 7];
            Buffer.BlockCopy(plain, from, output, 0, output.Length);
            return output;
        }

        /// <summary>
        /// Encrypt data with default key.
        /// </summary>
        public byte[] Encrypt(byte[] data) => Encrypt(data, new byte[16]);

        /// <summary>
        /// Decrypt data with default key.
        /// </summary>
        public byte[] Decrypt(byte[] data) => Decrypt(data, new byte[16]);

        private const long Delta = 2654435769L;
        private const long SumMax = (Delta << 4) & uint.MaxValue;

        private static readonly Random random = new Random();

        /// <summary>
        /// Write uint into array (Big-endian).
        /// </summary>
        private void WriteUInt32(byte[] data, int index, uint value)
        {
            data[index] = (byte)((value >> 24) & 0xFF);
            data[index + 1] = (byte)((value >> 16) & 0xFF);
            data[index + 2] = (byte)((value >> 8) & 0xFF);
            data[index + 3] = (byte)(value & 0xFF);
        }

        /// <summary>
        /// Read uint from array (Big-endian).
        /// </summary>
        private uint ReadUInt32(byte[] data, int index) =>
            ((uint)data[index] << 24) |
            ((uint)data[index + 1] << 16) |
            ((uint)data[index + 2] << 8) |
            data[index + 3];

        /// <summary>
        /// Get random byte.
        /// </summary>
        private byte RandomByte(int max = byte.MaxValue) => (byte)(random.Next() & max);

        /// <summary>
        /// Xor 8 bytes between 2 arrays.
        /// </summary>
        private byte[] Xor8(byte[] a, byte[] b, int ai = 0, int bi = 0)
        {
            if (bi < 0)
            {
                return a;
            }
            byte[] r = new byte[8];
            for (int i = 0; i < 8; ++i)
            {
                r[i] = (byte)(a[i + ai] ^ b[i + bi]);
            }
            return r;
        }

        /// <summary>
        /// TEA encrypt.
        /// </summary>
        private byte[] EnCipher(byte[] data, byte[] key)
        {
            byte[] array = new byte[8];
            //if (input.Length < 8 || key.Length < 16)
            //{
            //    return array;
            //}
            long sum = 0;
            long y = ReadUInt32(data, 0);
            long z = ReadUInt32(data, 4);
            long a = ReadUInt32(key, 0);
            long b = ReadUInt32(key, 4);
            long c = ReadUInt32(key, 8);
            long d = ReadUInt32(key, 12);
            for (int i = 0; i < 16; ++i)
            {
                sum += Delta;
                sum &= uint.MaxValue;
                y += ((z << 4) + a) ^ (z + sum) ^ ((z >> 5) + b);
                y &= uint.MaxValue;
                z += ((y << 4) + c) ^ (y + sum) ^ ((y >> 5) + d);
                z &= uint.MaxValue;
            }
            WriteUInt32(array, 0, (uint)y);
            WriteUInt32(array, 4, (uint)z);
            return array;
        }

        /// <summary>
        /// TEA decrypt.
        /// </summary>
        private byte[] DeCipher(byte[] data, byte[] key, long index = 0)
        {
            byte[] array = new byte[8];
            //if (input.Length < 8 || key.Length < 16)
            //{
            //    return array;
            //}
            long sum = SumMax;
            long y = ReadUInt32(data, (int)index);
            long z = ReadUInt32(data, (int)index + 4);
            long a = ReadUInt32(key, 0);
            long b = ReadUInt32(key, 4);
            long c = ReadUInt32(key, 8);
            long d = ReadUInt32(key, 12);
            for (int i = 0; i < 16; ++i)
            {
                z -= ((y << 4) + c) ^ (y + sum) ^ ((y >> 5) + d);
                z &= uint.MaxValue;
                y -= ((z << 4) + a) ^ (z + sum) ^ ((z >> 5) + b);
                y &= uint.MaxValue;
                sum -= Delta;
                sum &= uint.MaxValue;
            }
            WriteUInt32(array, 0, (uint)y);
            WriteUInt32(array, 4, (uint)z);
            return array;
        }
    }
}
