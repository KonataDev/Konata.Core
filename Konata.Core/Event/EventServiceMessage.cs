using System;

using Konata.Core.Packet;
using Konata.Core.Manager;
using Konata.Runtime.Network;
using Konata.Runtime.Base.Event;
using Konata.Utils.IO;
using Konata.Utils.Crypto;

namespace Konata.Core.Event
{
    public enum AuthFlag : byte
    {
        DefaultlyNo = 0x00,
        D2Authentication = 0x01,
        WtLoginExchange = 0x02,
    }

    public class EventServiceMessage : KonataEventArgs
    {
        private string _headUin;
        private byte[] _headExtra;

        private byte[] _keyData;

        private AuthFlag _authFlag;
        private PacketType _packetType;
        private byte[] _payloadData;
        private EventSsoFrame _payloadFrame;

        public string HeadUin { get => _headUin; }

        public EventSsoFrame Frame { get => _payloadFrame; }

        public byte[] FrameBytes { get => _payloadData; }

        public AuthFlag AuthFlag { get => _authFlag; }

        public PacketType MessagePktType { get => _packetType; }

        public bool IsServerResponse { get; private set; } = false;


        public static bool Build(EventServiceMessage toService, out byte[] output)
        {
            var write = new PacketBase();
            {
                write.PutUintBE((uint)toService.Frame.PacketType);
                write.PutByte((byte)toService._authFlag);

                write.PutBytes(toService._headExtra,
                   toService.Frame.PacketType == PacketType.TypeA ?
                   ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix : ByteBuffer.Prefix.None);

                write.PutByte(0x00);

                write.PutString(toService._headUin,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                if (toService._keyData == null)
                    write.PutByteBuffer(EventSsoFrame.Build(toService._payloadFrame));
                else
                    write.PutEncryptedBytes(EventSsoFrame.Build(toService._payloadFrame).GetBytes(),
                    TeaCryptor.Instance, toService._keyData);
            }

            output = write.GetBytes();
            return true;
        }

        public static bool Parse(SocketPackage package, out EventServiceMessage output)
        {
            var sigInfo = package.Owner.GetComponent<UserSigManager>();
            output = new EventServiceMessage();

            var read = new PacketBase(package.Data);
            {
                read.TakeUintBE(out var pktType);
                {
                    if (pktType != 0x0A && pktType != 0x0B)
                        return false;

                    output._packetType = (PacketType)pktType;
                }

                read.TakeByte(out var reqFlag);
                {
                    if (reqFlag != 0x00 && reqFlag != 0x01 && reqFlag != 0x02)
                        return false;
                    output._authFlag = (AuthFlag)reqFlag;
                }

                read.TakeByte(out var zeroByte);
                {
                    if (zeroByte != 0x00)
                        return false;
                }

                read.TakeString(out output._headUin,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
            }

            switch (output._authFlag)
            {
                case AuthFlag.DefaultlyNo:
                    read.TakeAllBytes(out output._payloadData);
                    break;
                case AuthFlag.D2Authentication:
                    read.TakeDecryptedBytes(out output._payloadData, TeaCryptor.Instance, sigInfo.D2Key);
                    break;
                case AuthFlag.WtLoginExchange:
                    read.TakeDecryptedBytes(out output._payloadData, TeaCryptor.Instance, sigInfo.ZeroKey);
                    break;
            }
            output.Owner = package.Owner;
            //TODO:
            //IsServerResponse?
            //output.IsServerResponse = true;

            return true;
        }

        public static bool Create(EventSsoFrame ssoFrame, AuthFlag reqFlag, uint reqUin,
            byte[] d2Token, byte[] d2Key, out EventServiceMessage output)
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

            output = new EventServiceMessage
            {
                _authFlag = reqFlag,
                _headUin = reqUin.ToString(),

                _keyData = keyData,
                _headExtra = headExtra,

                _payloadFrame = ssoFrame
            };

            return true;
        }

        public static bool Create(EventSsoFrame ssoFrame, AuthFlag reqFlag,
            uint reqUin, out EventServiceMessage output)
            => Create(ssoFrame, reqFlag, reqUin, null, null, out output);
    }
}
