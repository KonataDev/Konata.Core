using System;
using Konata.Msf.Crypto;

namespace Konata.Msf.Packets
{
    public class ToServiceMessage : Packet
    {
        private Header _header;
        private Body _body;

        // About packetType
        // 0x0A d2Token field always be d2Token
        // 0x0B d2Token replaced with ssoSeq

        public ToServiceMessage(uint packetType, uint uin, byte[] d2Token,
            byte[] d2Key, Packet packet)
            : base()
        {
            _body = new Body(packet, TeaCryptor.Instance, d2Key ?? new byte[16]);
            _header = new Header(packetType, (uint)(d2Token.Length == 0 ? 2 : 1),
                d2Token ?? new byte[0], uin.ToString());

            PutUintBE((uint)(_header.Length + _body.Length + 4));
            PutPacket(_header);
            PutPacket(_body);
        }

        private class Header : Packet
        {
            public Header(uint packetType, uint encryptType, byte[] d2Token, string uin)
            {
                PutUintBE(packetType);
                PutByte((byte)encryptType);

                PutUintBE((uint)(d2Token.Length + 4));
                PutBytes(d2Token);

                PutByte(0x00);

                PutUintBE((uint)(uin.Length + 4));
                PutString(uin);
            }
        }

        private class Body : Packet
        {
            public Body(Packet packet, ICryptor cryptor, byte[] cryptKey)
            {
                PutPacketEncrypted(packet, cryptor, cryptKey);
            }
        }
    }
}
