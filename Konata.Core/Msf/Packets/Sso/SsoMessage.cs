using System;
using Konata.Library.IO;
using Konata.Msf;
using Konata.Msf.Crypto;

namespace Konata.Msf.Packets
{
    public class SsoMessage : Packet
    {
        public Header ssoHeader;
        public ByteBuffer ssoWupBuffer;

        public SsoMessage(uint sequence, uint session, string command,
            byte[] tgtToken, ByteBuffer packet)
            : base()
        {
            ssoHeader = new Header(sequence, session, command, tgtToken);
            ssoWupBuffer = packet;

            PutUintBE((uint)(ssoHeader.Length + 4));
            PutPacket(ssoHeader);
            PutUintBE((uint)(ssoWupBuffer.Length + 4));
            PutBytes(ssoWupBuffer.GetBytes());
        }

        public SsoMessage(byte[] data, byte[] cryptKey)
            : base(data, TeaCryptor.Instance, cryptKey)
        {
            ssoHeader = new Header(GetBytes());
            ssoWupBuffer = new Packet(ssoHeader.TakeAllBytes(out byte[] _));
        }

        public SsoMessage(byte[] data)
            : base(data)
        {
            ssoHeader = new Header(GetBytes());
            ssoWupBuffer = new Packet(ssoHeader.TakeAllBytes(out byte[] _));
        }

        public class Header : Packet
        {
            private readonly byte[] token = { };
            private readonly byte[] unknownBytes0 = { };
            private readonly byte[] unknownBytes1 = { };
            private readonly string unknownString = $"||A{AppInfo.apkVersionName}.{AppInfo.appRevision}";

            public readonly uint ssoSequence;
            public readonly uint ssoSession;
            public readonly string ssoCommand;

            public Header(uint sequence, uint session, string command,
                byte[] tgtToken)
                : base()
            {
                ssoSequence = sequence;
                ssoSession = session;
                ssoCommand = command;

                if (tgtToken != null)
                {
                    token = tgtToken;
                }

                PutUintBE(ssoSequence);
                PutUintBE(AppInfo.subAppId);
                PutUintBE(AppInfo.subAppId);
                PutHexString("01 00 00 00 00 00 00 00 00 00 01 00");

                PutUintBE((uint)(token.Length + 4));
                PutBytes(token);

                PutUintBE((uint)(ssoCommand.Length + 4));
                PutString(ssoCommand);

                PutUintBE((uint)(4 + 4));
                PutUintBE((uint)ssoSession);

                PutUintBE((uint)(DeviceInfo.System.Imei.Length + 4));
                PutString(DeviceInfo.System.Imei);

                PutUintBE((uint)(unknownBytes0.Length + 4));
                PutBytes(unknownBytes0);

                PutUshortBE((ushort)(unknownString.Length + 2));
                PutString(unknownString);

                PutUintBE((uint)(unknownBytes1.Length + 4));
                PutBytes(unknownBytes1);
            }

            public Header(byte[] data) : base(data)
            {
                EatBytes(4);
                TakeUintBE(out ssoSequence);

                EatBytes(4);

                TakeBytes(out token, Prefix.Uint32 | Prefix.WithPrefix);
                TakeString(out ssoCommand, Prefix.Uint32 | Prefix.WithPrefix);

                EatBytes(4);
                TakeUintBE(out ssoSession);

                EatBytes(4);
            }
        }
    }
}
