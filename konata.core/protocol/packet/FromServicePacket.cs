using System;
using Konata.Protocol.Utils;
using Konata.Utils;
using Konata.Msf.Utils.Crypt;
using StreamReader = Konata.Utils.StreamReader;

namespace Konata.Protocol.Packet
{
    public class FromServicePacket : PacketBase
    {

        public SsoPacket _packet;

        public byte[] _ticket;

        public long _uin;

        public uint _packetType;

        public byte _encryptType;

        public byte[] _encryptKey = new byte[16];

        public FromServicePacket(byte[] fromServiceBytes)
        {
            // TryParse(fromServiceBytes);
        }

        //public override bool TryParse(byte[] data)
        //{
        //    StreamReader reader = new StreamReader(data);

        //    // 跳過頭部長度
        //    reader.Drop(4);

        //    // 包類型
        //    reader.TakeUInt32(out _packetType);

        //    // 加密類型
        //    reader.TakeUInt8(out _encryptType);

        //    // 未知
        //    reader.Drop(1);

        //    // 賬號字符串
        //    reader.TakeUInt32(out var length);
        //    reader.TakeString(out var uin, (int)length - 4);
        //    _uin = long.Parse(uin);

        //    // 剩下的數據
        //    reader.TakeRemainDecrypedBytes(out var ssoBody, new TeaCryptor(), _encryptKey);
        //    _packet = SsoPacketFactory.TryParse(ssoBody);

        //    return true;
        //}

    }
}
