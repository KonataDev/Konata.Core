using Konata.Msf;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets
{
    public class SsoMessage : Packet
    {
        public Header _header;
        public Packet _packet;

        public SsoMessage(uint seq, uint session, string command, Packet packet) : base()
        {
            _header = new Header(seq, session, command);
            _packet = packet;

            PutUintBE((uint)(_header.Length + 4));
            PutPacket(_header);
            PutUintBE((uint)(_packet.Length + 4));
            PutPacket(_packet);
        }

        public SsoMessage(byte[] data, byte[] cryptKey) :
            base(data, TeaCryptor.Instance, cryptKey)
        {
            _header = new Header(GetBytes());
            _packet = new Packet(_header.GetBytes());
        }

        public class Header : Packet
        {
            private readonly byte[] _extraData = { };
            private readonly byte[] _unknownBytes0 = { };
            private readonly byte[] _unknownBytes1 = { };
            private readonly string _unknownString = $"||A{AppInfo.apkVersionName}.{AppInfo.appRevision}";

            public readonly uint _ssoSequence;
            public readonly uint _ssoSession;
            public readonly string _ssoCommand;

            public Header(uint ssoSequence, uint ssoSession, string ssoCommand) : base()
            {
                _ssoSequence = ssoSequence;
                _ssoSession = ssoSession;
                _ssoCommand = ssoCommand;

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
            }

            public Header(byte[] data) : base(data)
            {
                EatBytes(4);
                TakeUintBE(out _ssoSequence);

                TakeBytes(out _extraData, Prefix.Uint16 | Prefix.WithPrefix);
                TakeString(out _ssoCommand, Prefix.Uint16 | Prefix.WithPrefix);

                EatBytes(4);
                TakeUintBE(out _ssoSession);

                EatBytes(4);
            }
        }
    }
}
