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

            // 跳過頭部長度
            reader.Drop(4);

            // 包類型
            reader.TakeUInt32(out _packetType);

            // 加密類型
            reader.TakeUInt8(out _encryptType);

            // 未知
            reader.Drop(1);

            // 賬號字符串
            reader.TakeUInt32(out var length);
            reader.TakeString(out var uin, (int)length - 4);

            // 剩下的數據
            reader.TakeRemainDecrypedBytes(out var ssoBody, new TeaCryptor(), _encryptKey);

            _uin = long.Parse(uin);
            _packet = new SsoPacket(ssoBody);

            return true;
        }

    }
}
