using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Konata.Utils.Crypto;

namespace Konata.Utils
{
    public class StreamBuilder
    {

        private readonly List<byte[]> _body = new List<byte[]>();

        private byte[] ProcessBytes(byte[] value, bool needFlipData, bool needPrefixLength,
            bool needLimitLength, int limitLength = 0)
        {

            if (value == null || (value.Length == 0 && !needPrefixLength))
            {
                return new byte[0];
            }

            byte[] data = new byte[value.Length];
            Array.Copy(value, data, value.Length);

            if (needLimitLength && data.Length > limitLength)
            {
                data = value.Take(limitLength).ToArray();
            }

            if (needFlipData && data.Length > 1)
            {
                data = data.Reverse().ToArray();
            }

            if (needPrefixLength)
            {
                PushUInt16((ushort)data.Length);
            }

            return data;
        }

        public void PushInt8(sbyte value)
        {
            PushBytes(new byte[] { (byte)value });
        }

        public void PushUInt8(byte value)
        {
            PushBytes(new byte[] { value });
        }

        public void PushBool(bool value, byte length = 1)
        {
            if (length > 8)
                throw new Exception("do not do that.");

            byte[] boolBytes = new byte[length];
            boolBytes[length - 1] = (byte)(value ? 1 : 0);

            PushBytes(boolBytes);
        }

        public void PushInt16(short value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushUInt16(ushort value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushInt32(int value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushUInt32(uint value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushInt64(long value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushUInt64(ulong value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushBytes(byte[] value, bool needFlipData = true, bool needPrefixLength = false,
            bool needLimitLength = false, int limitLength = 0)
        {
            _body.Add(ProcessBytes(value, needFlipData, needPrefixLength, needLimitLength, limitLength));
        }

        public void PushString(string value, bool needPrefixLength = true, bool needLimitLength = false, int limitLength = 0)
        {
            PushBytes(Encoding.UTF8.GetBytes(value), false, needPrefixLength, needLimitLength, limitLength);
        }

        public void PushHexString(string value, bool needFlipData = true, bool needPrefixLength = false,
            bool needLimitLength = false, int limitLength = 0)
        {
            PushBytes(Hex.HexStr2Bytes(value), needFlipData, needPrefixLength, needLimitLength, limitLength);
        }

        public byte[] GetBytes()
        {
            byte[] bytes = { };
            foreach (byte[] element in _body)
            {
                bytes = bytes.Concat(element).ToArray();
            }
            _body.Clear();
            return bytes;
        }

        public byte[] GetEncryptedBytes(ICryptor cryptor, byte[] cryptKey)
        {
            return cryptor.Encrypt(GetBytes(), cryptKey);
        }

        public int Length
        {
            get
            {
                int length = 0;
                foreach (byte[] element in _body)
                {
                    length += element.Length;
                }
                return length;
            }
        }

        public int Count => _body.Count;

    }
}
