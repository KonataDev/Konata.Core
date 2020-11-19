using System;
using Konata.Utils.Crypto;

namespace Konata.Model.Package.Tlv
{
    public class TlvPacker
    {
        private ushort _count;
        private readonly PacketBase _packet;

        public TlvPacker()
        {
            _count = 0;
            _packet = new PacketBase();
        }

        public void PutTlv(byte[] tlvData)
        {
            if (tlvData == null || tlvData.Length == 0)
            {
                return;
            }

            ++_count;
            _packet.PutBytes(tlvData);
        }

        public void PutTlv(Tlv tlvData)
        {
            if (tlvData == null)
            {
                return;
            }

            ++_count;
            _packet.PutTlv(tlvData);
        }


        public PacketBase GetPacket(bool prefixTlvCount)
        {
            var packet = new PacketBase();
            packet.PutUshortBE(_count);
            packet.PutPacket(_packet);
            return packet;
        }

        public byte[] GetBytes(bool prefixTlvCount)
        {
            return GetPacket(prefixTlvCount).GetBytes();
        }

        public byte[] GetEncryptedBytes(bool prefixTlvCount, ICryptor cryptor, byte[] cryptKey)
        {
            return cryptor.Encrypt(GetBytes(prefixTlvCount), cryptKey);
        }

    }
}
