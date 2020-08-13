using System;
using Konata.Utils;
using Konata.Utils.Crypto;

namespace Konata.Protocol.Packet
{
    public class ToServicePacket : PacketBase
    {

        private PacketBase _packet;

        private byte[] _ticket;

        private long _uin;

        private uint _packetType;

        private uint _encryptType;

        private byte[] _encryptKey = new byte[16];

        public ToServicePacket(PacketBase packet, uint packetType, uint encryptType, long uin)
        {
            _uin = uin;
            _packet = packet;
            _ticket = new byte[0];
            _packetType = packetType;
            _encryptType = encryptType;
        }

        public ToServicePacket(PacketBase packet, uint packetType, uint encryptType, byte[] encryptKey, long uin)
        {
            _uin = uin;
            _packet = packet;
            _ticket = new byte[0];
            _packetType = packetType;
            _encryptType = encryptType;
            _encryptKey = encryptKey;
        }

        public override byte[] GetBytes()
        {
            var uin = _uin.ToString();

            // 構建包躰頭部
            StreamBuilder builder = new StreamBuilder();
            builder.PushUInt32(_packetType);
            builder.PushUInt8((byte)_encryptType);

            builder.PushUInt32((uint)(_ticket.Length + 4));
            builder.PushBytes(_ticket, false);

            builder.PushUInt8(0x00);

            builder.PushUInt32((uint)(uin.Length + 4));
            builder.PushString(uin, false);

            // 構建整個包
            var packetHeader = builder.GetBytes();
            var packetBody = _packet.GetEncryptedBytes(new TeaCryptor(), _encryptKey);
            builder.PushUInt32((uint)(packetHeader.Length + packetBody.Length + 4));
            builder.PushBytes(packetHeader, false);
            builder.PushBytes(packetBody, false);

            return builder.GetBytes();
        }
    }
}
