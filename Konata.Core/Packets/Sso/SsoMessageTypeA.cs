using System;
using Konata.Library.IO;

namespace Konata.Packets.Sso
{
    public class SsoMessageTypeA : SsoMessage
    {
        public SsoMessageTypeA(uint sequence, string command,
            uint session, byte[] tgtoken, ByteBuffer payload)

            : base(sequence, command, session, RequestPktType.TypeA, (ByteBuffer w) =>
            {
                byte[] unknownBytes0 = { };
                byte[] unknownBytes1 = { };
                string unknownString = $"||A{AppInfo.apkVersionName}.{AppInfo.appRevision}";
                byte[] sessionBytes = ByteConverter.UInt32ToBytes(session, Endian.Big);

                var head = new ByteBuffer();
                {
                    head.PutUintBE(sequence);
                    head.PutUintBE(AppInfo.subAppId);
                    head.PutUintBE(AppInfo.subAppId);
                    head.PutHexString("01 00 00 00 00 00 00 00 00 00 01 00");

                    head.PutBytes(tgtoken ?? new byte[0],
                        ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutString(command,
                        ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutBytes(sessionBytes,
                        ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutString(DeviceInfo.System.Imei,
                        ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutBytes(unknownBytes0,
                        ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutString(unknownString,
                        ByteBuffer.Prefix.Uint16 | ByteBuffer.Prefix.WithPrefix);

                    head.PutBytes(unknownBytes1,
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
