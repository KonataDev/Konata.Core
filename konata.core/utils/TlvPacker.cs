using System;
using System.Linq;
using konata.Utils;
using Konata.Crypto;

namespace Konata.Utils
{
    public class TlvPacker
    {
        private int count = 0;
        private readonly StreamBuilder builder = new StreamBuilder();

        public void PushTlv(byte[] tlvData)
        {
            if (tlvData == null || tlvData.Length == 0)
                return;

            ++count;
            builder.PushBytes(tlvData, false);
        }

        public byte[] GetPacket(bool needPrefixTlvCount)
        {
            byte[] header = needPrefixTlvCount ? BitConverter.GetBytes(count).Reverse().ToArray() : new byte[0];
            byte[] tlvBytes = builder.GetPlainBytes();

            return header.Concat(tlvBytes).ToArray();
        }

        public byte[] GetEncryptedPacket(bool needPrefixTlvCount, IKonataCryptor cryptor, byte[] cryptKey)
        {
            return cryptor.Encrypt(GetPacket(needPrefixTlvCount), cryptKey);
        }

    }
}
