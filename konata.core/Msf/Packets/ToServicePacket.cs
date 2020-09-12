using System;
using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets
{
    public class ToServicePacket : Packet
    {

        private Packet _packet;

        private byte[] _ticket;

        private long _uin;

        private uint _packetType;

        private uint _encryptType;

        private byte[] _encryptKey = new byte[16];

        public ToServicePacket(Packet packet, uint packetType, uint encryptType, long uin)
        {
            _uin = uin;
            _packet = packet;
            _ticket = new byte[0];
            _packetType = packetType;
            _encryptType = encryptType;
        }

        public override byte[] GetBytes()
        {
            var uin = _uin.ToString();

            // 構建包躰頭部
            StreamBuilder builder = new StreamBuilder();
            builder.PutUintBE(_packetType);
            builder.PutByte((byte)_encryptType);

            builder.PutUintBE((uint)(_ticket.Length + 4));
            builder.PutBytes(_ticket);

            builder.PutByte(0x00);

            builder.PutUintBE((uint)(uin.Length + 4));
            builder.PutString(uin);

            // 構建整個包
            var packetHeader = builder.GetBytes();
            var packetBody = _packet.GetEncryptedBytes(new TeaCryptor(), _encryptKey);
            builder.PutUintBE((uint)(packetHeader.Length + packetBody.Length + 4));
            builder.PutBytes(packetHeader);
            builder.PutBytes(packetBody);

            return builder.GetBytes();
        }
    }
}
