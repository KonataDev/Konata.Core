using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Konata.Utils
{
    class TlvBuilder
    {
        private ushort cmd;
        private List<byte[]> body = new List<byte[]>();

        public TlvBuilder(ushort command)
        {
            cmd = command;
        }

        public void PushInt8(sbyte value)
        {
            PushBytes(new byte[] { (byte)value });
        }

        public void PushUInt8(byte value)
        {
            PushBytes(new byte[] { value });
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

        public void PushString(string value, bool IncludePrefixByteLength = true)
        {
            byte[] stringByte = Encoding.UTF8.GetBytes(value);

            if (IncludePrefixByteLength)
            {
                PushInt16((short)stringByte.Length);
            }

            PushBytes(stringByte, false);
        }

        public void PushBytes(byte[] value, bool flip = true)
        {
            body.Add(flip ? value.Reverse().ToArray() : value);
        }

        public byte[] GetPacket()
        {
            byte[] _tlv_body = new byte[0];

            foreach (byte[] i in body)
            {
                _tlv_body = _tlv_body.Concat(i).ToArray();
            }

            byte[] _tlv_cmd = BitConverter.GetBytes(cmd).Reverse().ToArray();
            byte[] _tlv_length = BitConverter.GetBytes((short)_tlv_body.Length).Reverse().ToArray();

            return _tlv_cmd.Concat(_tlv_length).Concat(_tlv_body).ToArray();
        }


    }

}
