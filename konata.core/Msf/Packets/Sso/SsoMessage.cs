using Konata.Msf;
using Konata.Utils;

namespace Konata.Msf.Packets
{
    public class SsoMessage : Packet
    {
        private Header _header;
        public Packet _packet;

        public string _ssoCommand;

        public SsoMessage(uint seq, uint session, string command, Packet packet)
        {
            _header = new Header(seq, session, command);
            _packet = packet;
        }

        public override byte[] GetBytes()
        {
            PutUintBE((uint)(_header.Length + 4));
            PutPacket(_header);
            PutUintBE((uint)(_packet.Length + 4));
            PutPacket(_packet);

            return base.GetBytes();
        }

        private class Header : Packet
        {
            private static readonly byte[] _extraData = { };
            private static readonly byte[] _unknownBytes0 = { };
            private static readonly byte[] _unknownBytes1 = { };
            private static readonly string _unknownString = $"||A{AppInfo.apkVersionName}.{AppInfo.appRevision}";

            public Header(uint seq, uint session, string command)
            {
                PutUintBE(seq);
                PutUintBE(AppInfo.subAppId);
                PutUintBE(AppInfo.subAppId);
                PutHexString("01 00 00 00 00 00 00 00 00 00 01 00");

                PutUintBE((uint)(_extraData.Length + 4));
                PutBytes(_extraData);

                PutUintBE((uint)(command.Length + 4));
                PutString(command);

                PutUintBE((uint)(4 + 4));
                PutUintBE((uint)session);

                PutUintBE((uint)(DeviceInfo.System.Imei.Length + 4));
                PutString(DeviceInfo.System.Imei);

                PutUintBE((uint)(_unknownBytes0.Length + 4));
                PutBytes(_unknownBytes0);

                PutUshortBE((ushort)(_unknownString.Length + 2));
                PutString(_unknownString);

                PutUintBE((uint)(_unknownBytes1.Length + 4));
                PutBytes(_unknownBytes1);
            }
        }
    }
}
