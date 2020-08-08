using System;
using System.Linq;
using Konata.Protocol.Packet.Tlvs;
using Konata.Utils;
using Konata.Utils.Crypto;

namespace Konata.Protocol.Utils
{
    public class TlvPacker
    {
        private short count = 0;
        private readonly StreamBuilder builder = new StreamBuilder();

        public void PushTlv(byte[] tlvData)
        {
            if (tlvData == null || tlvData.Length == 0)
                return;

            ++count;
            builder.PushBytes(tlvData, false);
        }

        public void PushTlv(TlvBase tlvData)
        {
            if (tlvData == null)
                return;

            ++count;
            builder.PushBytes(tlvData.GetBytes(), false);
        }

        public byte[] GetPacket(bool needPrefixTlvCount)
        {
            byte[] header = needPrefixTlvCount ? BitConverter.GetBytes(count).Reverse().ToArray() : new byte[0];
            byte[] tlvBytes = builder.GetPlainBytes();

            return header.Concat(tlvBytes).ToArray();
        }

        public byte[] GetEncryptedPacket(bool needPrefixTlvCount, ICryptor cryptor, byte[] cryptKey)
        {
            return cryptor.Encrypt(GetPacket(needPrefixTlvCount), cryptKey);
        }

    }
}
