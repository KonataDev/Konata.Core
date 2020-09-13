using System;
using System.Linq;
using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlv
{
    public class TlvPacker
    {
        private ushort _count;
        private readonly Packet _packet;

        public TlvPacker()
        {
            _count = 0;
            _packet = new Packet();
        }

        public void PushTlv(byte[] tlvData)
        {
            if (tlvData == null || tlvData.Length == 0)
                return;

            ++_count;
            _packet.PutBytes(tlvData);
        }

        public void PutTlv(TlvBase tlvData)
        {
            if (tlvData == null)
                return;

            ++_count;
            _packet.PutTlv(tlvData);
        }


        public Packet GetPacket(bool prefixTlvCount)
        {
            var packet = new Packet();
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
