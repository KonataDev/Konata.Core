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
                byte[] unknownBytes0 = { };
                byte[] sessionBytes = ByteConverter.UInt32ToBytes(session, Endian.Big);

                w.PutString(command,
                    ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);

                w.PutBytes(sessionBytes,
                    ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);

                w.PutBytes(unknownBytes0,
                    ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);

                w.PutByteBuffer(payload,
                    ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);
            })
        {

        }
    }
}
