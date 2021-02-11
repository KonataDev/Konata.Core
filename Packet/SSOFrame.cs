using System;

using Konata.Utils.IO;

namespace Konata.Core.Packet
{
    public enum PacketType : uint
    {
        TypeA = 0x0A,
        TypeB = 0x0B
    }

    public class SSOFrame
    {
        private uint _session;
        private int _sequence;
        private string _command;
        private ByteBuffer _payload;
        private PacketType _packetType;
        private byte[] _tgtoken;

        public uint Session { get => _session; }

        public string Command { get => _command; }

        public int Sequence { get => _sequence; }

        public byte[] Tgtoken { get => _tgtoken; }

        public ByteBuffer Payload { get => _payload; }

        public PacketType PacketType { get => _packetType; }

        public bool IsServerResponse { get; private set; }

        public static bool Parse(ServiceMessage fromService, out SSOFrame output)
        {
            output = new SSOFrame
            {
                _packetType = fromService.MessagePktType
            };

            var read = new ByteBuffer(fromService.FrameBytes);
            {
                read.TakeUintBE(out var length);
                {
                    if (length > read.Length)
                        return false;
                }

                read.TakeIntBE(out output._sequence);

                read.TakeUintBE(out var zeroUint);
                {
                    if (zeroUint != 0)
                        return false;
                }

                read.TakeBytes(out var unknownBytes,
                        ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                read.TakeString(out output._command,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                read.TakeBytes(out var session,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
                {
                    if (session.Length != 4)
                        return false;

                    output._session = ByteConverter.BytesToUInt32(session, 0);
                }

                read.TakeBoolBE(out var isCompressed, 4);
                {
                    read.TakeBytes(out var bytes,
                        ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
                    {
                        output._payload = new ByteBuffer
                            (isCompressed ? Deflate.Decompress(bytes) : bytes);
                    }
                }
            }

            //TODO:
            //IsServerResponse?
            //output.IsServerResponse = true;
            return true;
        }

        public static ByteBuffer Build(SSOFrame ssoFrame)
        {
            byte[] unknownBytes0 = { };
            byte[] unknownBytes1 = { };
            string unknownString = $"||A{AppInfo.ApkVersionName}.{AppInfo.AppRevision}";
            byte[] sessionBytes = ByteConverter.UInt32ToBytes(ssoFrame._session, Endian.Big);

            var write = new PacketBase();
            var head = new PacketBase();
            {
                if (ssoFrame.PacketType == PacketType.TypeA)
                {
                    head.PutIntBE(ssoFrame._sequence);
                    head.PutUintBE(AppInfo.SubAppId);
                    head.PutUintBE(AppInfo.SubAppId);
                    head.PutHexString("01 00 00 00 00 00 00 00 00 00 01 00");

                    head.PutBytes(ssoFrame._tgtoken ?? new byte[0],
                            ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutString(ssoFrame._command,
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
                else if (ssoFrame.PacketType == PacketType.TypeB)
                {
                    head.PutString(ssoFrame._command,
                           ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutBytes(sessionBytes,
                           ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                    head.PutBytes(unknownBytes0,
                           ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
                }
            }
            write.PutByteBuffer(head,
                    ByteBuffer.Prefix.WithPrefix | ByteBuffer.Prefix.Uint32);

            write.PutByteBuffer(ssoFrame.Payload,
                ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

            return write;
        }

        public static bool Create(string command, PacketType pktType, int sequence,
            byte[] tgtoken, uint session, ByteBuffer payload, out SSOFrame ssoFrame)
        {
            ssoFrame = new SSOFrame
            {
                _command = command,
                _sequence = sequence,
                _session = session,
                _packetType = pktType,
                _payload = payload,
                _tgtoken = tgtoken
            };

            return true;
        }

        public static bool Create(string command, PacketType pktType, int sequence,
            uint session, ByteBuffer payload, out SSOFrame ssoFrame)
          => Create(command, pktType, sequence, null, session, payload, out ssoFrame);
    }
}
