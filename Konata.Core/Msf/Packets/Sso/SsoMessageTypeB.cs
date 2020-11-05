using System;
using Konata.Library.IO;

namespace Konata.Msf.Packets.Sso
{
    public class SsoMessageTypeB : SsoMessage
    {
        public SsoMessageTypeB(uint sequence, string command, uint session,
                  ByteBuffer payload)

            : base(sequence, command, session, RequestPktType.TypeB, (ByteBuffer w) =>
            {
                var head = new ByteBuffer();
                {
                    byte[] unknownBytes0 = { };
                    byte[] sessionBytes = ByteConverter.UInt32ToBytes(session, Endian.Big);

                    head.PutString(command,
                       ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutBytes(sessionBytes,
                       ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutBytes(unknownBytes0,
                       ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutByteBuffer(payload,
                       ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
                }
                w.PutByteBuffer(head,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
                w.PutByteBuffer(payload,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
            })
        {

        }
    }
}
