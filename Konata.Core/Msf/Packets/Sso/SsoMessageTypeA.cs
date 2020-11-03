using System;
using Konata.Library.IO;

namespace Konata.Msf.Packets.Sso
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
                        ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);

                    head.PutString(command,
                        ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);

                    head.PutBytes(sessionBytes,
                        ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);

                    head.PutString(DeviceInfo.System.Imei,
                        ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);

                    head.PutBytes(unknownBytes0,
                        ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);

                    head.PutString(unknownString,
                        ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint16);

                    head.PutBytes(unknownBytes1,
                        ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);
                }

                w.PutByteBuffer(head,
                    ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);
                w.PutByteBuffer(payload,
                    ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);
            })
        {

        }
    }
}
