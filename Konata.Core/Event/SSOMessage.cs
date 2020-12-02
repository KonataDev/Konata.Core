using System;
using System.Collections.Generic;

using Konata.Utils.IO;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public enum RequestPktType : uint
    {
        TypeA = 0x0A,
        TypeB = 0x0B
    }

    public class SSOMessage : KonataEventArgs
    {
        private uint _session;
        private uint _sequence;
        private string _command;
        private ByteBuffer _payload;
        private RequestPktType _packetType;

        public uint Session { get => _session; }

        public string Command { get => _command; }

        public uint Sequence { get => _sequence; }

        public ByteBuffer Payload { get => _payload; }

        public RequestPktType PacketType { get => _packetType; }

        private SSOMessage() { }

        public static bool Parse(ServiceMessage serviceMsg, out SSOMessage ssoMsg)
        {
            ssoMsg = new SSOMessage();
            ssoMsg.Owner = serviceMsg.Owner;
            ssoMsg._packetType = serviceMsg.MessagePktType;

            var r = new ByteBuffer(serviceMsg.Payload);
            {
                r.TakeUintBE(out var length);
                {
                    if (length > r.Length)
                        return false;
                }

                r.TakeUintBE(out ssoMsg._sequence);

                r.TakeUintBE(out var zeroUint);
                {
                    if (zeroUint != 0)
                        return false;
                }

                r.TakeBytes(out var unknownBytes,
                        ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                r.TakeString(out ssoMsg._command,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                r.TakeBytes(out var session,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
                {
                    if (session.Length != 4)
                        return false;

                    ssoMsg._session = ByteConverter.BytesToUInt32(session, 0);
                }

                r.TakeUintBE(out zeroUint);
                {
                    if (zeroUint != 0)
                        return false;
                }

                r.TakeBytes(out var bytes,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
                {
                    ssoMsg._payload = new ByteBuffer(bytes);
                }
            }

            return true;
        }

        public static bool PackType0x0A()
            => throw new NotImplementedException();

        public static bool PackType0x0B()
            => throw new NotImplementedException();
    }
}
