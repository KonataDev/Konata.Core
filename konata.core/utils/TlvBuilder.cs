using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Utils
{
    class TlvBuilder
    {
        private short cmd_;
        private List<byte[]> body_;

        public TlvBuilder(short command)
        {
            cmd_ = command;
        }

        public void PushInt16(Int16 value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushUint16(UInt16 value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushInt32(Int32 value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushUint32(UInt32 value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushInt64(Int64 value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushUint64(UInt64 value)
        {
            PushBytes(BitConverter.GetBytes(value));
        }

        public void PushHexStr(string value)
        {

        }

        public void PushBytes(byte[] value)
        {
            body_.Add(value);
        }

        public byte[] GetPacket()
        {
            byte[] _tlv_body = new byte[0];

            foreach (byte[] i in body_)
            {
                _tlv_body = _tlv_body.Concat(i).ToArray();
            }

            byte[] _tlv_cmd = BitConverter.GetBytes(cmd_);
            byte[] _tlv_length = BitConverter.GetBytes((short)_tlv_body.Length);

            return _tlv_length.Concat(_tlv_cmd).Concat(_tlv_body).ToArray();
        }


    }

}
