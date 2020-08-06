using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Konata.Utils.Crypto;

namespace Konata.Utils
{
    public class StreamBuilder
    {

        private readonly List<byte[]> body = new List<byte[]>();

        private byte[] ProcessBytes(byte[] value, bool needFlipData, bool needPrefixLength,
            bool needLimitLength, int limitLength = 0)
        {
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

        public void PushBool(bool value, int length = 1)
        {
            if (length < 0 || length > 8)
                throw new Exception("do not do that.");

            byte[] boolBytes = new byte[length];
            boolBytes[length - 1] = (byte)(value ? 1 : 0);

            PushBytes(new byte[] { (byte)(value ? 1 : 0) });
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
        public void PushBytes(byte[] value, bool needFlipData = true, bool needPrefixLength = false, bool needLimitLength = false, int limitLength = 0)
        {
            body.Add(ProcessBytes(value, needFlipData, needPrefixLength, needLimitLength, limitLength));
        }

        public void PushString(string value, bool needPrefixLength = true, bool needLimitLength = false, int limitLength = 0)
        {
            PushBytes(Encoding.UTF8.GetBytes(value), false, needPrefixLength, needLimitLength, limitLength);
        }

        public byte[] GetPlainBytes()
        {
            byte[] bytes = { };
            foreach (byte[] element in body)
            {
                bytes = bytes.Concat(element).ToArray();
            }
            return bytes;
        }

        public byte[] GetEncryptedBytes(IKonataCryptor cryptor, byte[] cryptKey)
        {
            return cryptor.Encrypt(GetPlainBytes(), cryptKey);
        }

    }
}
