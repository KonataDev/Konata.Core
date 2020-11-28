using Konata.Model.Packet.Sso;
using Konata.Runtime.Base.Event;
using Konata.Utils.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.EventArgs
{
    public class SSOMessage : KonataEventArgs
    {
        private string _ssoCommand;
        private uint _ssoSequence;

        public RequestPktType SSOPktType { get; set; }
        public uint SSOSession { get; set; }
        public string SSOCommand
        {
            get => _ssoCommand;
        }
        public uint SSOSequence
        {
            get => _ssoSequence;
        }
        public ByteBuffer Payload { get; set; }

        private SSOMessage() { }

        public static bool ToSSOMessage(KonataEventArgs arg,byte[] data,RequestPktType type,out SSOMessage msg)
        {
            msg = new SSOMessage();
            msg.SSOPktType = type;
            msg.Receiver = arg.Receiver;
            var buffer = new ByteBuffer(data);
            buffer.TakeUintBE(out var length);
            {
                if (length > buffer.Length)
                    return false;
            }

            buffer.TakeUintBE(out msg._ssoSequence);

            buffer.TakeUintBE(out var zeroUint);
            {
                if (zeroUint != 0)
                    return false;
            }

            buffer.TakeBytes(out var unknownBytes,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

            buffer.TakeString(out msg._ssoCommand,
                ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

            buffer.TakeBytes(out var session,
                ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
            {
                if (session.Length != 4)
                    return false;

                msg.SSOSession = ByteConverter.BytesToUInt32(session, 0);
            }

            buffer.TakeUintBE(out zeroUint);
            {
                if (zeroUint != 0)
                    return false;
            }

            buffer.TakeBytes(out var bytes,
                ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
            {
                msg.Payload = new ByteBuffer(bytes);
            }

            return true;
        }
    }
}
