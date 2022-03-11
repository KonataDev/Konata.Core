using Konata.Core.Utils.IO;
using Konata.Core.Utils.Crypto;

// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Packets.Oicq
{
    public enum OicqStatus
    {
        OK = 0,

        DoVerifySmsCaptcha = 160,
        DoVerifySliderCaptcha = 2,

        DoVerifyDeviceLock = 204,
        DoVerifyDeviceLockViaSmsNewArea = 239,

        PreventByIncorrectPassword = 1,
        PreventByReceiveIssue = 3,
        PreventByTokenExpired = 15,
        PreventByAccountBanned = 40,
        PreventByOperationTimeout = 155,
        PreventBySmsSentFailed = 162,
        PreventByIncorrectSmsCode = 163,
        PreventByLoginDenied = 167,
        PreventByHighRiskEnvironment = 237,
    }

    public enum OicqEncryptMethod
    {
        ECDH7 = 0x07,
        ECDH135 = 0x87,
    }

    public class OicqRequest : PacketBase
    {
        public delegate void OicqBodyWriter(PacketBase writer);

        public OicqRequest(ushort command, uint uin, OicqEncryptMethod method,
            byte[] shareKey, byte[] randKey, byte[] publicKey, OicqBodyWriter writer) : base()
        {
            var body = new PacketBase();

            PutByte(0x02); // Head of 0x02
            {
                EnterBarrier(Prefix.Uint16, Endian.Big, 4);
                {
                    PutUshortBE(8001); // oicqVersion
                    PutUshortBE(command); // oicqCommand
                    PutUshortBE(1);
                    PutUintBE(uin);
                    PutByte(0x03);
                    PutByte((byte) method);
                    PutByte(0x00); // 0x00
                    PutUintBE(2);
                    PutUintBE(AppInfo.AppClientVersion);
                    PutUintBE(0);

                    writer.Invoke(body);
                    PutOicqRequestBody(body, shareKey, randKey, publicKey);
                }
                LeaveBarrier();
            }
            PutByte(0x03); // End with 0x03
        }

        private void PutOicqRequestBody(PacketBase body, byte[] shareKey,
            byte[] randKey, byte[] publicKey)
        {
            PutUshortBE(0x0101);
            PutBytes(randKey);
            PutUshortBE(0x0102);
            PutBytes(publicKey, Prefix.Uint16);
            PutPacketEncrypted(body, TeaCryptor.Instance, shareKey);
        }
    }
}
