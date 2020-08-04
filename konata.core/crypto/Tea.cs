using System;

namespace Konata.Crypto
{
    public class Tea
    {
        private long _contextStart;
        private long _crypt;
        private long _preCrypt;
        private bool _header;
        private byte[] _key = new byte[16];
        private byte[] _out;
        private long _padding;
        private byte[] _plain;
        private byte[] _prePlain;
        private readonly Random _random = new Random();

        private byte[] WriteBigEndian(byte[] array, int offset, uint input)
        {
            if (offset + 4 <= array.Length)
            {
                array[offset] = (byte)((input >> 24) & 0xFF);
                array[offset + 1] = (byte)((input >> 16) & 0xFF);
                array[offset + 2] = (byte)((input >> 8) & 0xFF);
                array[offset + 3] = (byte)(input & 0xFF);
            }
            return array;
        }

        private uint GetUnsignedInt(byte[] array, int offset) => ((uint)array[offset] << 24) | ((uint)array[offset + 1] << 16) | ((uint)array[offset + 2] << 8) | array[offset + 3];

        private int Rand() => _random.Next();

        private byte[] Decipher(byte[] arrayIn, byte[] arrayKey, int offset = 0)
        {
            byte[] array = new byte[8];
            if (arrayIn.Length < 8 || arrayKey.Length < 16)
            {
                return array;
            }
            long sum = 3816266640L;
            long delta = 2654435769L;
            long y = GetUnsignedInt(arrayIn, offset);
            long z = GetUnsignedInt(arrayIn, offset + 4);
            long a = GetUnsignedInt(arrayKey, 0);
            long b = GetUnsignedInt(arrayKey, 4);
            long c = GetUnsignedInt(arrayKey, 8);
            long d = GetUnsignedInt(arrayKey, 12);
            for (int i = 0; i < 16; ++i)
            {
                z -= ((y << 4) + c) ^ (y + sum) ^ ((y >> 5) + d);
                z &= uint.MaxValue;
                y -= ((z << 4) + a) ^ (z + sum) ^ ((z >> 5) + b);
                y &= uint.MaxValue;
                sum -= delta;
                sum &= uint.MaxValue;
            }
            WriteBigEndian(array, 0, (uint)y);
            WriteBigEndian(array, 4, (uint)z);
            return array;
        }

        private byte[] Encipher(byte[] arrayIn, byte[] arrayKey, int offset = 0)
        {
            byte[] array = new byte[8];
            if (arrayIn.Length < 8 || arrayKey.Length < 16)
            {
                return array;
            }
            long sum = 0;
            long delta = 2654435769L;
            long y = GetUnsignedInt(arrayIn, offset);
            long z = GetUnsignedInt(arrayIn, offset + 4);
            long a = GetUnsignedInt(arrayKey, 0);
            long b = GetUnsignedInt(arrayKey, 4);
            long c = GetUnsignedInt(arrayKey, 8);
            long d = GetUnsignedInt(arrayKey, 12);
            for (int i = 0; i < 16; ++i)
            {
                sum += delta;
                sum &= uint.MaxValue;
                y += ((z << 4) + a) ^ (z + sum) ^ ((z >> 5) + b);
                y &= uint.MaxValue;
                z += ((y << 4) + c) ^ (y + sum) ^ ((y >> 5) + d);
                z &= uint.MaxValue;
            }
            WriteBigEndian(array, 0, (uint)y);
            WriteBigEndian(array, 4, (uint)z);
            return array;
        }

        private void Encrypt8Bytes()
        {
            for (int i = 0; i < 8; ++i)
            {
                if (_header)
                {
                    _plain[i] = (byte)(_plain[i] ^ _prePlain[i]);
                }
                else
                {
                    _plain[i] = (byte)(_plain[i] ^ _out[_preCrypt + i]);
                }
            }

            byte[] array = Encipher(_plain, _key);
            for (int i = 0; i < 8; ++i)
            {
                _out[_crypt + i] = (byte)(array[i] ^ _prePlain[i]);
            }

            _plain.CopyTo(_prePlain, 0);
            _preCrypt = _crypt;
            _crypt += 8L;
            _header = false;
        }

        private bool Decrypt8Bytes(byte[] arrayIn, long offset = 0L)
        {
            for (int i = 0; i < 8; ++i)
            {
                if (_contextStart + i > arrayIn.Length - 1)
                {
                    return true;
                }
                _prePlain[i] = (byte)(_prePlain[i] ^ arrayIn[offset + _crypt + i]);
            }

            try
            {
                _prePlain = Decipher(_prePlain, _key);
            }
            catch
            {
                return false;
            }

            int num = _prePlain.Length - 1;
            _contextStart += 8L;
            _crypt += 8L;
            return true;
        }

        public byte[] Encrypt(byte[] arrayIn, byte[] arrayKey, long offset)
        {
            _plain = new byte[8];
            _prePlain = new byte[8];
            _padding = 0L;
            _crypt = _preCrypt = 0L;
            _key = arrayKey;
            _header = true;
            int pos = (arrayIn.Length + 10) % 8;
            if (pos != 0)
            {
                pos = 8 - pos;
            }

            _out = new byte[arrayIn.Length + pos + 10];
            _plain[0] = (byte)((Rand() & 0xF8) | pos);
            for (int i = 1; i <= pos; i++)
            {
                _plain[i] = (byte)(Rand() & 0xFF);
            }

            pos++;
            _padding = 1L;
            while (_padding < 3)
            {
                if (pos < 8)
                {
                    _plain[pos] = (byte)(Rand() & 0xFF);
                    _padding++;
                    pos++;
                }
                else if (pos == 8)
                {
                    Encrypt8Bytes();
                }
            }

            int num = (int)offset;
            long num2 = arrayIn.Length;
            while (num2 > 0)
            {
                if (pos < 8)
                {
                    _plain[pos] = arrayIn[num];
                    num++;
                    pos++;
                    num2--;
                }
                else if (pos == 8)
                {
                    Encrypt8Bytes();
                }
            }

            _padding = 1L;
            while (_padding < 9)
            {
                if (pos < 8)
                {
                    _plain[pos] = 0;
                    pos++;
                    _padding++;
                }
                else if (pos == 8)
                {
                    Encrypt8Bytes();
                }
            }

            return _out;
        }

        public byte[] Encrypt(byte[] arrayIn, byte[] arrayKey)
        {
            byte[] array = null;
            int num = 0;
            while (array == null && num < 2)
            {
                try
                {
                    array = Encrypt(arrayIn, arrayKey, 0L);
                }
                catch { }
                num++;
            }

            return array;
        }

        public byte[] Encrypt(byte[] arrayIn, int pos, int len, byte[] arrayKey)
        {
            if (arrayIn == null || arrayKey == null)
            {
                return null;
            }

            byte[] part = new byte[len];
            Array.Copy(arrayIn, pos, part, 0, len);

            return Encrypt(part, arrayKey);
        }

        // Token: 0x06000637 RID: 1591 RVA: 0x00026210 File Offset: 0x00024410
        public byte[] Decrypt(byte[] inData, byte[] key)
        {
            byte[] result = new byte[0];
            try
            {
                result = Decrypt(inData, key, 0L);
            }
            catch { }
            return result;
        }

        public byte[] Decrypt(byte[] inData, int pos, int len, byte[] key)
        {
            if (inData == null || key == null)
            {
                return null;
            }

            byte[] part = new byte[len];
            Array.Copy(inData, pos, part, 0, len);

            return Decrypt(part, key);
        }

        public byte[] Decrypt(byte[] arrayIn, byte[] arrayKey, long offset)
        {
            byte[] result = new byte[0];
            if (arrayIn.Length < 16 || arrayIn.Length % 8 != 0)
            {
                return result;
            }

            byte[] array = new byte[offset + 8];
            arrayKey.CopyTo(_key, 0);
            _crypt = _preCrypt = 0L;
            _prePlain = Decipher(arrayIn, arrayKey, (int)offset);
            int pos = _prePlain[0] & 7;
            long num = arrayIn.Length - pos - 10;
            if (num <= 0)
            {
                return result;
            }

            _out = new byte[num];
            _preCrypt = 0L;
            _crypt = 8L;
            _contextStart = 8L;
            pos++;
            _padding = 1L;
            while (_padding < 3)
            {
                if (pos < 8)
                {
                    pos++;
                    _padding++;
                }
                else if (pos == 8)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = arrayIn[i];
                    }

                    if (!Decrypt8Bytes(arrayIn, offset))
                    {
                        return result;
                    }
                }
            }

            long num2 = 0L;
            while (num != 0)
            {
                if (pos < 8)
                {
                    _out[num2] = (byte)(array[offset + _preCrypt + pos] ^ _prePlain[pos]);
                    num2++;
                    num--;
                    pos++;
                }
                else if (pos == 8)
                {
                    array = arrayIn;
                    _preCrypt = _crypt - 8;
                    if (!Decrypt8Bytes(arrayIn, offset))
                    {
                        return result;
                    }
                }
            }

            for (_padding = 1L; _padding <= 7; _padding++)
            {
                if (pos < 8)
                {
                    if ((array[offset + _preCrypt + pos] ^ _prePlain[pos]) != 0)
                    {
                        return result;
                    }

                    pos++;
                }
                else if (pos == 8)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = arrayIn[i];
                    }

                    _preCrypt = _crypt;
                    if (!Decrypt8Bytes(arrayIn, offset))
                    {
                        return result;
                    }
                }
            }

            return _out;
        }
    }
}
