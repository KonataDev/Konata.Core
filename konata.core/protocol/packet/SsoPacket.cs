using Konata.Utils;
using Konata.Protocol.Packet.Oicq;

using SsoCommand = Konata.Protocol.SsoServiceCmd.Command;

namespace Konata.Protocol.Packet
{
    public class SsoPacket : PacketBase
    {
        public uint _ssoSquence;

        public uint _ssoSessionId;

        public OicqRequest _oicqRequest;

        public SsoCommand _ssoCommand;

        public SsoPacket(byte[] fromServiceBytes)
        {
            TryParse(fromServiceBytes);
        }

        public SsoPacket(uint seq, uint session, SsoCommand command, OicqRequest request)
        {
            _ssoSquence = seq;
            _ssoSessionId = session;
            _ssoCommand = command;
            _oicqRequest = request;
        }

        public override byte[] GetBytes()
        {
            var extraData = new byte[0];
            var ssoCommand = SsoServiceCmd.ToString(_ssoCommand);
            var unknownBytes0 = new byte[0];
            var unknownBytes1 = new byte[0];
            var unknownString = $"||A{AppInfo.apkVersionName}.{AppInfo.appRevision}";

            // 構建頭部包躰
            StreamBuilder builder = new StreamBuilder();
            builder.PushUInt32(_ssoSquence);
            builder.PushUInt32(AppInfo.subAppId);
            builder.PushUInt32(AppInfo.subAppId);
            builder.PushHexString("01 00 00 00 00 00 00 00 00 00 01 00", false);

            builder.PushUInt32((uint)(extraData.Length + 4));
            builder.PushBytes(extraData, false);

            builder.PushUInt32((uint)(ssoCommand.Length + 4));
            builder.PushString(ssoCommand, false);

            builder.PushUInt32((uint)(4 + 4));
            builder.PushUInt32((uint)_ssoSessionId);

            builder.PushUInt32((uint)(DeviceInfo.System.Imei.Length + 4));
            builder.PushString(DeviceInfo.System.Imei, false);

            builder.PushUInt32((uint)(unknownBytes0.Length + 4));
            builder.PushBytes(unknownBytes0, false);

            builder.PushUInt16((ushort)(unknownString.Length + 2));
            builder.PushString(unknownString, false);

            builder.PushUInt32((uint)(unknownBytes1.Length + 4));
            builder.PushBytes(unknownBytes1, false);

            // 構建整個包
            var ssoHeader = builder.GetBytes();
            var ssoRequest = _oicqRequest.GetBytes();
            builder.PushUInt32((uint)(ssoHeader.Length + 4));
            builder.PushBytes(ssoHeader, false);
            builder.PushUInt32((uint)(ssoRequest.Length + 4));
            builder.PushBytes(ssoRequest, false);

            return builder.GetBytes();
        }

        public override bool TryParse(byte[] data)
        {

            StreamReader reader = new StreamReader(data);

            // 跳過頭部長度
            reader.Drop(4);

            // sso包序號
            reader.TakeUInt32(out _ssoSquence);

            // 未知4字節 總是0
            reader.Drop(4);

            // 未知4字節 和 0長數據
            reader.TakeUInt32(out var length);
            reader.Drop((int)length - 4);

            // sso指令
            reader.TakeUInt32(out length);
            reader.TakeString(out var command, (int)length - 4);
            _ssoCommand = SsoServiceCmd.TryParse(command);

            // cookie
            reader.TakeUInt32(out length);
            reader.TakeUInt32(out _ssoSessionId);

            // 未知4字節 總是0
            reader.Drop(4);

            // 剩下的數據
            reader.TakeRemainBytes(out var requestBody);
            return OicqRequest.TryParse(requestBody, out _oicqRequest);
        }
    }
}
