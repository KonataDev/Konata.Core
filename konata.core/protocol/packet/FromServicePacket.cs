using System;
using Konata.Utils;
using Konata.Utils.Crypto;
using StreamReader = Konata.Utils.StreamReader;

namespace Konata.Protocol.Packet
{
    public class FromServicePacket : PacketBase
    {

        protected SsoPacket _packet;

        protected byte[] _ticket;

        protected long _uin;

        protected uint _packetType;

        protected byte _encryptType;

        protected byte[] _encryptKey = new byte[16];

        public FromServicePacket(byte[] fromServiceBytes)
        {
            TryParse(fromServiceBytes);
        }

        public override bool TryParse(byte[] data)
        {
            StreamReader reader = new StreamReader(data);

            reader.Drop(4);
            reader.TakeUInt32(out _packetType);
            reader.TakeUInt8(out _encryptType);
            reader.TakeUInt32(out var length);
            reader.TakeBytes(out _ticket, (int)length - 4);
            reader.Drop(1);
            reader.TakeUInt32(out length);
            reader.TakeString(out var uin, (int)length - 4);
            reader.TakeRemainDecrypedBytes(out var ssoBody, new TeaCryptor(), _encryptKey);

            _uin = long.Parse(uin);
            _packet = new SsoPacket(ssoBody);

            return true;
        }

    }
}
