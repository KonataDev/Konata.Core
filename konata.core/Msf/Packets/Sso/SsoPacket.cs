using Konata.Msf;
using Konata.Utils;

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
            var builder = new StreamBuilder();
            builder.PutUintBE(_ssoSquence);
            builder.PutUintBE(AppInfo.subAppId);
            builder.PutUintBE(AppInfo.subAppId);
            builder.PutHexString("01 00 00 00 00 00 00 00 00 00 01 00");

            builder.PutUintBE((uint)(extraData.Length + 4));
            builder.PutBytes(extraData);

            builder.PutUintBE((uint)(ssoCommand.Length + 4));
            builder.PutString(ssoCommand, false);

            builder.PutUintBE((uint)(4 + 4));
            builder.PutUintBE((uint)_ssoSessionId);

            builder.PutUintBE((uint)(DeviceInfo.System.Imei.Length + 4));
            builder.PutString(DeviceInfo.System.Imei);

            builder.PutUintBE((uint)(unknownBytes0.Length + 4));
            builder.PutBytes(unknownBytes0);

            builder.PutUshortBE((ushort)(unknownString.Length + 2));
            builder.PutString(unknownString);

            builder.PutUintBE((uint)(unknownBytes1.Length + 4));
            builder.PutBytes(unknownBytes1);

            // 構建整個包
            var ssoHeader = builder.GetBytes();
            var ssoRequest = _packet.GetBytes();
            PutUintBE((uint)(ssoHeader.Length + 4));
            PutBytes(ssoHeader);
            PutUintBE((uint)(ssoRequest.Length + 4));
            PutBytes(ssoRequest);

            return base.GetBytes();
        }

    }
}
