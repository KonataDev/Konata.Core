using System;
using Konata.Utils.IO;

namespace Konata.Core.Packet.Sso
{
    public enum RequestPktType : uint
    {
        TypeA = 0x0A,
        TypeB = 0x0B
    }

    public delegate void SsoPayloadWriter(ByteBuffer writer);

    public class SsoMessage
    {
        private uint ssoSession;
        private uint ssoSequence;
        private string ssoCommand;
        private ByteBuffer ssoPayload;
        private RequestPktType ssoPktType;

        public SsoMessage(string ssoCmd, uint ssoseq, uint ssoSess,
            RequestPktType type, SsoPayloadWriter writer)
            : base()
        {
            ssoCommand = ssoCmd;
            ssoSequence = ssoseq;
            ssoSession = ssoSess;

            ssoPktType = type;
            ssoPayload = new ByteBuffer();
            {
                writer(ssoPayload);
            }
        }

        public SsoMessage(byte[] data, RequestPktType type)
            : base()
        {
            ssoPktType = type;
            var r = new ByteBuffer(data);
            {
                r.TakeUintBE(out var length);
                {
                    if (length > r.Length)
                        throw new Exception("Invalid sso message.");
                }

                r.TakeUintBE(out ssoSequence);

                r.TakeUintBE(out var zeroUint);
                {
                    if (zeroUint != 0)
                        throw new Exception("Invalid sso message.");
                }

                r.TakeBytes(out var unknownBytes,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                r.TakeString(out ssoCommand,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                r.TakeBytes(out var session,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
                {
                    if (session.Length != 4)
                        throw new Exception("Invalid sso message.");

                    ssoSession = ByteConverter.BytesToUInt32(session, 0);
                }

                r.TakeUintBE(out zeroUint);
                {
                    if (zeroUint != 0)
                        throw new Exception("Invalid sso message.");
                }

                r.TakeBytes(out var bytes,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
                {
                    ssoPayload = new ByteBuffer(bytes);
                }
            }
        }

        public RequestPktType GetPacketType() =>
            ssoPktType;

        public byte[] GetPayload() =>
            ssoPayload.GetBytes();

        public uint GetSequence() =>
            ssoSequence;

        public uint GetSession() =>
            ssoSession;

        public string GetCommand() =>
            ssoCommand;
    }
}
