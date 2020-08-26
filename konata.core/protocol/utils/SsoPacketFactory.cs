using System;
using System.Text;
using Konata.Utils;
using Konata.Protocol.Packet;
using Konata.Protocol.Packet.Oicq;
using SsoCommand = Konata.Protocol.SsoServiceCmd.Command;

namespace Konata.Protocol.Utils
{
    public static class SsoPacketFactory
    {
        private static uint _ssoSequence = 85600;
        private static uint _ssoSessionId = 0xBCA2DA01;

        public static SsoPacket New(SsoCommand command, OicqRequest request)
        {
            ++_ssoSequence;
            return new SsoPacket(_ssoSequence, _ssoSessionId, command, request);
        }

        //public static SsoPacket TryParse(byte[] data)
        //{
        //    StreamReader reader = new StreamReader(data);

        //    // 跳過頭部長度
        //    reader.Drop(4);

        //    // sso包序號
        //    reader.TakeUInt32(out var ssoSquence);

        //    // 未知4字節 總是0
        //    reader.Drop(4);

        //    // 未知4字節 和 0長數據
        //    reader.TakeUInt32(out var length);
        //    reader.Drop((int)length - 4);

        //    // sso指令
        //    reader.TakeUInt32(out length);
        //    reader.TakeString(out var command, (int)length - 4);
        //    var ssoCommand = SsoServiceCmd.TryParse(command);

        //    // cookie
        //    reader.TakeUInt32(out length);
        //    reader.TakeUInt32(out var ssoSessionId);

        //    // 未知4字節 總是0
        //    reader.Drop(4);

        //    // 剩下的數據
        //    reader.TakeRemainBytes(out var requestBody);

        //    _ssoSequence = ssoSquence;
        //    _ssoSessionId = ssoSessionId;

        //    return new SsoPacket(ssoSquence, _ssoSessionId, ssoCommand);
        //}
    }
}
