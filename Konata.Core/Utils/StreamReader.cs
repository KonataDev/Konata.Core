using System;
using System.Linq;
using System.Text;
using Konata.Msf.Utils.Crypt;

namespace Konata.Utils
{
    public class StreamReader
    {

        private byte[] _body;
        private int _position;

        public StreamReader(byte[] data)
        {
            _body = data;
            _position = 0;
        }

        private byte[] ReadBytes(int length)
        {
            if (_body.Length - _position < length)
                throw new Exception("no enough data");

            byte[] data = _body.Skip(_position).Take(length).ToArray();
            _position += length;

            return data;
        }

        public StreamReader TakeUInt8(out byte value)
        {
            value = ReadBytes(1)[0];
            return this;
        }

        public StreamReader TakeUInt16(out ushort value)
        {
            value = BitConverter.ToUInt16(ReadBytes(2).Reverse().ToArray(), 0);
            return this;
        }

        public StreamReader TakeUInt32(out uint value)
        {
            value = BitConverter.ToUInt32(ReadBytes(4).Reverse().ToArray(), 0);
            return this;
        }

        public StreamReader TakeUInt64(out ulong value)
        {
            value = BitConverter.ToUInt64(ReadBytes(8).Reverse().ToArray(), 0);
            return this;
        }

        public StreamReader TakeBytes(out byte[] value)
        {
            TakeUInt16(out var length);
            value = ReadBytes(length);
            return this;
        }

        public StreamReader TakeBytes(out byte[] value, int length)
        {
            value = ReadBytes(length);
            return this;
        }

        public StreamReader TakeString(out string value)
        {
            TakeUInt16(out var length);
            value = Encoding.UTF8.GetString(ReadBytes(length));
            return this;
        }

        public StreamReader TakeString(out string value, int length)
        {
            value = Encoding.UTF8.GetString(ReadBytes(length));
            return this;
        }

        public StreamReader TakeRemainBytes(out byte[] value)
        {
            value = ReadBytes(_body.Length - _position);
            return this;
        }

        public StreamReader TakeRemainDecrypedBytes(out byte[] value, ICryptor cryptor, byte[] key)
        {
            TakeRemainBytes(out var data);
            value = cryptor.Decrypt(data, key);
            return this;
        }

        public StreamReader Drop(int length)
        {
            ReadBytes(length);
            return this;
        }

    }
}
