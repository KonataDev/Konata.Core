using Konata.Utils;

using SsoCommand = Konata.Protocol.SsoServiceCmd.Command;

namespace Konata.Msf.Packets
{
    public class SsoPacket : Packet
    {
        public uint _ssoSquence;

        public uint _ssoSessionId;

        public Packet _packet;

        public SsoCommand _ssoCommand;

        public SsoPacket(uint seq, uint session, SsoCommand command, Packet packet)
        {
            _ssoSquence = seq;
            _ssoSessionId = session;
            _ssoCommand = command;
            _packet = packet;
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

    }
}
