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
        private long _pos;
        private byte[] _prePlain;
        private readonly Random _random = new Random();

        private byte[] CopyMemory(byte[] arr, int arrIndex, long input)
        {
            if (arrIndex + 4 > arr.Length)
            {
                return arr;
            }

            arr[arrIndex] = (byte)(input & 0xFF);
            arr[arrIndex + 1] = (byte)((input >> 8) & 0xFF);
            arr[arrIndex + 2] = (byte)((input >> 16) & 0xFF);
            arr[arrIndex + 3] = (byte)((input >> 24) & 0xFF);

            // arr[arrIndex + 3] = (byte)((input & 0xFF000000U) >> 24);
            // arr[arrIndex + 2] = (byte)((input & 0xFF0000) >> 16);
            // arr[arrIndex + 1] = (byte)((input & 0xFF00) >> 8);
            // arr[arrIndex] = (byte)(input & 0xFF);
            // arr[arrIndex] &= byte.MaxValue;
            // arr[arrIndex + 1] &= byte.MaxValue;
            // arr[arrIndex + 2] &= byte.MaxValue;
            // arr[arrIndex + 3] &= byte.MaxValue;
            return arr;
        }

        private long GetUnsignedInt(byte[] arrayIn, int offset, int len)
        {
            long num = 0L;
            int num2 = len <= 8 ? offset + len : offset + 8;
            for (int i = offset; i < num2; i++)
            {
                num <<= 8;
                num |= (ushort)(arrayIn[i] & 0xFF);
            }

            return (num & uint.MaxValue) | (num >> 32);
        }

        private long Rand()
        {
            return _random.Next();
        }

        private byte[] Decipher(byte[] arrayIn, byte[] arrayKey, long offset = 0L)
        {
            byte[] arr = new byte[24];
            byte[] array = new byte[8];
            if (arrayIn.Length < 8)
            {
                return array;
            }

            if (arrayKey.Length < 16)
            {
                return array;
            }

            long num = 3816266640L;
            num &= uint.MaxValue;
            long num2 = 2654435769L;
            num2 &= uint.MaxValue;
            long num3 = GetUnsignedInt(arrayIn, (int)offset, 4);
            long num4 = GetUnsignedInt(arrayIn, (int)offset + 4, 4);
            long unsignedInt = GetUnsignedInt(arrayKey, 0, 4);
            long unsignedInt2 = GetUnsignedInt(arrayKey, 4, 4);
            long unsignedInt3 = GetUnsignedInt(arrayKey, 8, 4);
            long unsignedInt4 = GetUnsignedInt(arrayKey, 12, 4);
            for (int i = 1; i <= 16; i++)
            {
                num4 -= ((num3 << 4) + unsignedInt3) ^ (num3 + num) ^ ((num3 >> 5) + unsignedInt4);
                num4 &= uint.MaxValue;
                num3 -= ((num4 << 4) + unsignedInt) ^ (num4 + num) ^ ((num4 >> 5) + unsignedInt2);
                num3 &= uint.MaxValue;
                num -= num2;
                num &= uint.MaxValue;
            }

            arr = CopyMemory(arr, 0, num3);
            arr = CopyMemory(arr, 4, num4);
            array[0] = arr[3];
            array[1] = arr[2];
            array[2] = arr[1];
            array[3] = arr[0];
            array[4] = arr[7];
            array[5] = arr[6];
            array[6] = arr[5];
            array[7] = arr[4];
            return array;
        }

        private byte[] Encipher(byte[] arrayIn, byte[] arrayKey, long offset = 0L)
        {
            byte[] array = new byte[8];
            byte[] arr = new byte[24];
            if (arrayIn.Length < 8 || arrayKey.Length < 16)
            {
                return array;
            }

            long num = 0L;
            long num2 = 2654435769L;
            num2 &= uint.MaxValue;
            long num3 = GetUnsignedInt(arrayIn, (int)offset, 4);
            long num4 = GetUnsignedInt(arrayIn, (int)offset + 4, 4);
            long unsignedInt = GetUnsignedInt(arrayKey, 0, 4);
            long unsignedInt2 = GetUnsignedInt(arrayKey, 4, 4);
            long unsignedInt3 = GetUnsignedInt(arrayKey, 8, 4);
            long unsignedInt4 = GetUnsignedInt(arrayKey, 12, 4);
            for (int i = 1; i <= 16; i++)
            {
                num += num2;
                num &= uint.MaxValue;
                num3 += ((num4 << 4) + unsignedInt) ^ (num4 + num) ^ ((num4 >> 5) + unsignedInt2);
                num3 &= uint.MaxValue;
                num4 += ((num3 << 4) + unsignedInt3) ^ (num3 + num) ^ ((num3 >> 5) + unsignedInt4);
                num4 &= uint.MaxValue;
            }

            arr = CopyMemory(arr, 0, num3);
            arr = CopyMemory(arr, 4, num4);
            array[0] = arr[3];
            array[1] = arr[2];
            array[2] = arr[1];
            array[3] = arr[0];
            array[4] = arr[7];
            array[5] = arr[6];
            array[6] = arr[5];
            array[7] = arr[4];
            return array;
        }

        private void Encrypt8Bytes()
        {
            for (_pos = 0L; _pos < 8; _pos++)
            {
                if (_header)
                {
                    _plain[_pos] = (byte)(_plain[_pos] ^ _prePlain[_pos]);
                }
                else
                {
                    _plain[_pos] = (byte)(_plain[_pos] ^ _out[_preCrypt + _pos]);
                }
            }

            byte[] array = Encipher(_plain, _key);
            for (int i = 0; i <= 7; i++)
            {
                _out[_crypt + i] = array[i];
            }

            for (_pos = 0L; _pos <= 7; _pos++)
            {
                _out[_crypt + _pos] = (byte)(_out[_crypt + _pos] ^ _prePlain[_pos]);
            }

            _plain.CopyTo(_prePlain, 0);
            _preCrypt = _crypt;
            _crypt += 8L;
            _pos = 0L;
            _header = false;
        }

        private bool Decrypt8Bytes(byte[] arrayIn, long offset = 0L)
        {
            for (_pos = 0L; _pos <= 7; _pos++)
            {
                if (_contextStart + _pos > arrayIn.Length - 1)
                {
                    return true;
                }

                _prePlain[_pos] = (byte)(_prePlain[_pos] ^ arrayIn[offset + _crypt + _pos]);
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
            _pos = 0L;
            return true;
        }

        public byte[] Encrypt(byte[] arrayIn, byte[] arrayKey, long offset)
        {
            _plain = new byte[8];
            _prePlain = new byte[8];
            _pos = 1L;
            _padding = 0L;
            _crypt = _preCrypt = 0L;
            _key = arrayKey;
            _header = true;
            _pos = 2L;
            _pos = (arrayIn.Length + 10) % 8;
            if (_pos != 0)
            {
                _pos = 8 - _pos;
            }

            _out = new byte[arrayIn.Length + _pos + 10];
            _plain[0] = (byte)((Rand() & 0xF8) | _pos);
            for (int i = 1; i <= _pos; i++)
            {
                _plain[i] = (byte)(Rand() & 0xFF);
            }

            _pos++;
            _padding = 1L;
            while (_padding < 3)
            {
                if (_pos < 8)
                {
                    _plain[_pos] = (byte)(Rand() & 0xFF);
                    _padding++;
                    _pos++;
                }
                else if (_pos == 8)
                {
                    Encrypt8Bytes();
                }
            }

            int num = (int)offset;
            long num2 = arrayIn.Length;
            while (num2 > 0)
            {
                if (_pos < 8)
                {
                    _plain[_pos] = arrayIn[num];
                    num++;
                    _pos++;
                    num2--;
                }
                else if (_pos == 8)
                {
                    Encrypt8Bytes();
                }
            }

            _padding = 1L;
            while (_padding < 9)
            {
                if (_pos < 8)
                {
                    _plain[_pos] = 0;
                    _pos++;
                    _padding++;
                }
                else if (_pos == 8)
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
                catch
                {
                }

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
            catch
            {
            }

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
            _prePlain = Decipher(arrayIn, arrayKey, offset);
            _pos = _prePlain[0] & 7;
            long num = arrayIn.Length - _pos - 10;
            if (num <= 0)
            {
                return result;
            }

            _out = new byte[num];
            _preCrypt = 0L;
            _crypt = 8L;
            _contextStart = 8L;
            _pos++;
            _padding = 1L;
            while (_padding < 3)
            {
                if (_pos < 8)
                {
                    _pos++;
                    _padding++;
                }
                else if (_pos == 8)
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
                if (_pos < 8)
                {
                    _out[num2] = (byte)(array[offset + _preCrypt + _pos] ^ _prePlain[_pos]);
                    num2++;
                    num--;
                    _pos++;
                }
                else if (_pos == 8)
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
                if (_pos < 8)
                {
                    if ((array[offset + _preCrypt + _pos] ^ _prePlain[_pos]) != 0)
                    {
                        return result;
                    }

                    _pos++;
                }
                else if (_pos == 8)
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
