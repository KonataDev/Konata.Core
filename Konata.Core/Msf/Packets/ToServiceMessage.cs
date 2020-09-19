using System;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets
{
    public class ToServiceMessage : Packet
    {
        private Body _body;
        private Header _header;

        public ToServiceMessage(uint packetType, uint encryptType, uint uin, Packet packet)
        {
            _body = new Body(packet, new TeaCryptor(), new byte[16]);
            _header = new Header(packetType, encryptType, new byte[0], uin.ToString());

            PutUintBE((uint)(_header.Length + _body.Length + 4));
            PutPacket(_header);
            PutPacket(_body);
        }

        private class Header : Packet
        {
            public Header(uint packetType, uint encryptType, byte[] ticket, string uin)
            {
                PutUintBE(packetType);
                PutByte((byte)encryptType);

                PutUintBE((uint)(ticket.Length + 4));
                PutBytes(ticket);

                PutByte(0x00);

                PutUintBE((uint)(uin.Length + 4));
                PutString(uin);
            }
        }

        private class Body : Packet
        {
            public Body(Packet packet, ICryptor cryptor, byte[] cryptKey)
            {
                PutEncryptedPacket(packet, cryptor, cryptKey);
            }
        }
    }
}
