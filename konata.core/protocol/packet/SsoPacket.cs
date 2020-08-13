using Konata.Protocol.Packet.Oicq;
using Konata.Utils;

using SsoCommand = Konata.Protocol.SsoServiceCmd.Command;

namespace Konata.Protocol.Packet
{
    public class SsoPacket : PacketBase
    {
        public uint _ssoSquence { get; }

        public uint _ssoSessionId { get; }

        public OicqRequest _oicqRequest { get; }

        public SsoCommand _ssoCommand { get; }

        public SsoPacket(uint seq, uint session, SsoCommand command, OicqRequest request)
        {
            _ssoSquence = seq;
            _ssoSessionId = session;
            _ssoCommand = command;
            _oicqRequest = request;
        }

        public override byte[] GetBytes()
        {
            var length = 1;
            var extraData = new byte[0];
            var ssoCommand = SsoServiceCmd.ToString(_ssoCommand);
            var unknownBytes0 = new byte[0];
            var unknownBytes1 = new byte[0];
            var unknownString = $"||{AppInfo.appBuildVer}.{AppInfo.appRevision}";
            var requestBytes = _oicqRequest.GetBytes();

            // 構建頭部包躰
            StreamBuilder builder = new StreamBuilder();
            builder.PushInt32(length);

            builder.PushUInt32(_ssoSquence);
            builder.PushUInt32(AppInfo.appId);
            builder.PushUInt32(AppInfo.subAppId);
            builder.PushHexString("01 00 00 00 00 00 00 00 00 00 01 00", false);

            builder.PushUInt32((uint)(extraData.Length + 4));
            builder.PushBytes(extraData, false);

            builder.PushUInt32((uint)(ssoCommand.Length + 4));
            builder.PushString(ssoCommand, false);

            builder.PushUInt32((uint)(0x04 + 0x04));
            builder.PushUInt32((uint)_ssoSessionId);

            builder.PushUInt32((uint)(DeviceInfo.System.Imei.Length + 4));
            builder.PushString(DeviceInfo.System.Imei, false);

            builder.PushUInt32((uint)(unknownBytes0.Length + 4));
            builder.PushBytes(unknownBytes0, false);

            builder.PushUInt16((ushort)(unknownString.Length + 4));
            builder.PushString(unknownString);

            builder.PushUInt32((uint)(unknownBytes1.Length + 4));
            builder.PushBytes(unknownBytes1, false);

            builder.PushUInt32((uint)requestBytes.Length);
            builder.PushBytes(requestBytes, false);

            return builder.GetPlainBytes();
        }
    }
}
