using Konata.Msf;
using Konata.Utils;

namespace Konata.Msf.Packets
{
    public class SsoMessage : Packet
    {
        public Header _header;
        public Packet _packet;

        public SsoMessage(uint seq, uint session, string command, Packet packet)
        {
            _header = new Header(seq, session, command);
            _packet = packet;
        }

        public SsoMessage(byte[] data)
        {
            _header = new Header(data);
            _packet = new Packet(_header.GetBytes());
        }

        public override byte[] GetBytes()
        {
            PutUintBE((uint)(_header.Length + 4));
            PutPacket(_header);
            PutUintBE((uint)(_packet.Length + 4));
            PutPacket(_packet);

            return base.GetBytes();
        }

        public class Header : Packet
        {
            private static readonly byte[] _extraData = { };
            private static readonly byte[] _unknownBytes0 = { };
            private static readonly byte[] _unknownBytes1 = { };
            private static readonly string _unknownString = $"||A{AppInfo.apkVersionName}.{AppInfo.appRevision}";

            public readonly uint _ssoSequence;
            public readonly uint _ssoSession;
            public readonly string _ssoCommand;

            public Header(uint ssoSequence, uint session, string command)
            {
                _ssoSequence = ssoSequence;
                _ssoSession = session;
                _ssoCommand = command;
            }

            public Header(byte[] data)
            {

            }

            public override byte[] GetBytes()
            {
                PutUintBE(_ssoSequence);
                PutUintBE(AppInfo.subAppId);
                PutUintBE(AppInfo.subAppId);
                PutHexString("01 00 00 00 00 00 00 00 00 00 01 00");

                PutUintBE((uint)(_extraData.Length + 4));
                PutBytes(_extraData);

                PutUintBE((uint)(_ssoCommand.Length + 4));
                PutString(_ssoCommand);

                PutUintBE((uint)(4 + 4));
                PutUintBE((uint)_ssoSession);

                PutUintBE((uint)(DeviceInfo.System.Imei.Length + 4));
                PutString(DeviceInfo.System.Imei);

                PutUintBE((uint)(_unknownBytes0.Length + 4));
                PutBytes(_unknownBytes0);

                PutUshortBE((ushort)(_unknownString.Length + 2));
                PutString(_unknownString);

                PutUintBE((uint)(_unknownBytes1.Length + 4));
                PutBytes(_unknownBytes1);

                return base.GetBytes();
            }
        }
    }
}
