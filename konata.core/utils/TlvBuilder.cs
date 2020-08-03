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

        public void PushString(string value, bool needPrefixLength = true, bool needLimitLength = false, int limitLength = 0)
        {
            PushBytes(Encoding.UTF8.GetBytes(value), false, needPrefixLength, needLimitLength, limitLength);
        }

        public void PushBytes(byte[] value, bool needFlipData = true, bool needPrefixLength = false, bool needLimitLength = false, int limitLength = 0)
        {
            byte[] data = value;

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

            body.Add(data);
        }

        public byte[] GetPacket()
        {
            byte[] tlvBody = new byte[0];

            foreach (byte[] element in body)
            {
                tlvBody = tlvBody.Concat(element).ToArray();
            }

            byte[] tlvCmd = BitConverter.GetBytes(cmd).Reverse().ToArray();
            byte[] tlvLength = BitConverter.GetBytes((short)tlvBody.Length).Reverse().ToArray();

            return tlvCmd.Concat(tlvLength).Concat(tlvBody).ToArray();
        }


    }

}
