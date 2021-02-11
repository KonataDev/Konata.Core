using System;

using Konata.Utils.IO;
using Konata.Utils.Crypto;
using Konata.Core.Service;

namespace Konata.Core.Packet
{
    public enum AuthFlag : byte
    {
        DefaultlyNo = 0x00,
        D2Authentication = 0x01,
        WtLoginExchange = 0x02,
    }

    public class ServiceMessage
    {
        private string _headUin;
        private byte[] _headExtra;

        private byte[] _keyData;

        private AuthFlag _authFlag;
        private PacketType _packetType;
        private byte[] _payloadData;
        private SSOFrame _payloadFrame;

        public string HeadUin { get => _headUin; }

        public SSOFrame Frame { get => _payloadFrame; }

        public byte[] FrameBytes { get => _payloadData; }

        public AuthFlag AuthFlag { get => _authFlag; }

        public PacketType MessagePktType { get => _packetType; }

        public bool IsServerResponse { get; private set; } = false;


        public static bool Build(ServiceMessage toService, out byte[] output)
        {
            var body = new PacketBase();
            var write = new PacketBase();
            {
                body.PutUintBE((uint)toService.Frame.PacketType);
                body.PutByte((byte)toService._authFlag);

                body.PutBytes(toService._headExtra,
                   toService.Frame.PacketType == PacketType.TypeA ?
                   ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix : ByteBuffer.Prefix.None);

                body.PutByte(0x00);

                body.PutString(toService._headUin,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                if (toService._keyData == null)
                    body.PutByteBuffer(SSOFrame.Build(toService._payloadFrame));
                else
                    body.PutEncryptedBytes(SSOFrame.Build(toService._payloadFrame).GetBytes(),
                    TeaCryptor.Instance, toService._keyData);
            }
            write.PutByteBuffer(body,
                ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

            output = write.GetBytes();
            return true;
        }

        public static bool Parse(byte[] buffer, SignInfo signinfo, out ServiceMessage output)
            => Parse(buffer, signinfo.D2Key, signinfo.ZeroKey, out output);

        public static bool Parse(byte[] buffer, byte[] d2Key, byte[] zeroKey, out ServiceMessage output)
        {
            output = new ServiceMessage();

            var read = new PacketBase(buffer);
            {
                // Packet Length
                read.EatBytes(4);

                // Packet Type
                read.TakeUintBE(out var pktType);
                {
                    if (pktType != 0x0A && pktType != 0x0B)
                        return false;

                    output._packetType = (PacketType)pktType;
                }

                // Auth Flag
                read.TakeByte(out var reqFlag);
                {
                    if (reqFlag != 0x00 && reqFlag != 0x01 && reqFlag != 0x02)
                        return false;
                    output._authFlag = (AuthFlag)reqFlag;
                }

                // Fixed zero
                read.TakeByte(out var zeroByte);
                {
                    if (zeroByte != 0x00)
                        return false;
                }

                // Uin
                read.TakeString(out output._headUin,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
            }

            switch (output._authFlag)
            {
                case AuthFlag.DefaultlyNo:
                    read.TakeAllBytes(out output._payloadData);
                    break;
                case AuthFlag.D2Authentication:
                    read.TakeDecryptedBytes(out output._payloadData, TeaCryptor.Instance, d2Key);
                    break;
                case AuthFlag.WtLoginExchange:
                    read.TakeDecryptedBytes(out output._payloadData, TeaCryptor.Instance, zeroKey);
                    break;
            }


            //TODO:
            //IsServerResponse?
            //output.IsServerResponse = true;

            return output._payloadData != null;
        }

        public static bool Create(SSOFrame ssoFrame, AuthFlag reqFlag, uint reqUin,
            SignInfo signinfo, out ServiceMessage output)
            => Create(ssoFrame, reqFlag, reqUin, signinfo.D2Token, signinfo.D2Key, out output);

        public static bool Create(SSOFrame ssoFrame, AuthFlag reqFlag,
            uint reqUin, out ServiceMessage output)
            => Create(ssoFrame, reqFlag, reqUin, null, null, out output);

        public static bool Create(SSOFrame ssoFrame, AuthFlag reqFlag, uint reqUin,
            byte[] d2Token, byte[] d2Key, out ServiceMessage output)
        {
            var keyData = new byte[0];
            var headExtra = new byte[0];

            switch (reqFlag)
            {
                case AuthFlag.DefaultlyNo:
                    keyData = null;
                    headExtra = new byte[0];
                    break;
                case AuthFlag.D2Authentication:
                    keyData = d2Key;
                    headExtra = d2Token;

                    if (ssoFrame.PacketType == PacketType.TypeB)
                        headExtra = ByteConverter.Int32ToBytes(ssoFrame.Sequence, Endian.Big);
                    break;
                case AuthFlag.WtLoginExchange:
                    keyData = new byte[16];
                    headExtra = new byte[0];
                    break;
            }

            output = new ServiceMessage
            {
                _authFlag = reqFlag,
                _headUin = reqUin.ToString(),

                _keyData = keyData,
                _headExtra = headExtra,

                _payloadFrame = ssoFrame
            };

            return true;
        }
    }
}
