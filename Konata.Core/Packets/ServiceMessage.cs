using System;
using Konata.Library.IO;
using Konata.Crypto;
using Konata.Packets.Sso;

namespace Konata.Packets
{
    public enum RequestFlag : byte
    {
        DefaultEmpty = 0x00,
        D2Authentication = 0x01,
        WtLoginExchange = 0x02,
    }

    public class ServiceMessage
    {
        private string headUin;
        private byte[] headExtra;

        private byte[] keyData;

        private byte[] svcPayload;
        private RequestFlag svcFlag;
        private RequestPktType svcPktType;

        public ServiceMessage(RequestFlag reqFlag, RequestPktType pktType,
            uint ssoSeq, byte[] d2Token = null, byte[] d2Key = null)
            : base()
        {
            svcFlag = reqFlag;
            svcPktType = pktType;

            switch (reqFlag)
            {
                case RequestFlag.DefaultEmpty:
                    keyData = null;
                    headExtra = new byte[0];
                    break;
                case RequestFlag.D2Authentication:
                    keyData = d2Key;
                    headExtra = d2Token;

                    if (pktType == RequestPktType.TypeB)
                        headExtra = ByteConverter.UInt32ToBytes(ssoSeq, Endian.Big);
                    break;
                case RequestFlag.WtLoginExchange:
                    keyData = new byte[16];
                    headExtra = new byte[0];
                    break;
            }
        }

        public ServiceMessage(byte[] fromService)
        {
            svcPayload = fromService;
            if (!TeardownFromService())
                throw new Exception("Invalid packet received.");
        }

        public ServiceMessage SetPayload(byte[] payload)
        {
            svcPayload = payload;
            return this;
        }

        public ServiceMessage SetUin(uint uin)
        {
            headUin = uin.ToString();
            return this;
        }

        public byte[] GetPayload(byte[] key)
        {
            if (key == null)
                return svcPayload;

            var decrypt = TeaCryptor.Instance.Decrypt(svcPayload, key);

            if (decrypt == null)
                throw new Exception("Invalid sso message. Can not decrypt.");

            return decrypt;
        }

        public uint GetUin() =>
            uint.Parse(headUin);

        public RequestPktType GetPacketType() =>
            svcPktType;

        public RequestFlag GetPacketFlag() =>
            svcFlag;

        public ByteBuffer BuildToService()
        {
            var toService = new PacketBase();
            {
                toService.PutUintBE((uint)svcPktType);
                toService.PutByte((byte)svcFlag);

                toService.PutBytes(headExtra,
                    svcPktType == RequestPktType.TypeA ?
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix :
                    ByteBuffer.Prefix.None);

                toService.PutByte(0x00);

                toService.PutString(headUin,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                if (keyData == null)
                    toService.PutBytes(svcPayload);
                else
                    toService.PutEncryptedBytes(svcPayload,
                        TeaCryptor.Instance, keyData);
            }
            return toService;
        }

        private bool TeardownFromService()
        {
            var fromService = new PacketBase(svcPayload);
            {
                fromService.TakeUintBE(out var pktType);
                {
                    if (pktType != 0x0A && pktType != 0x0B)
                        return false;

                    svcPktType = (RequestPktType)pktType;
                }

                fromService.TakeByte(out var reqFlag);
                {
                    if (reqFlag != 0x00 && reqFlag != 0x01 && reqFlag != 0x02)
                        return false;
                    svcFlag = (RequestFlag)reqFlag;
                }

                fromService.TakeByte(out var zeroByte);
                {
                    if (zeroByte != 0x00)
                        return false;
                }

                fromService.TakeString(out headUin,
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

                fromService.TakeAllBytes(out svcPayload);

                return true;
            }
        }
    }
}
