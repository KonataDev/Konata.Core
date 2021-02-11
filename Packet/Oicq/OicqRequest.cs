using System;
using Konata.Utils.Crypto;
using Konata.Utils.IO;

namespace Konata.Core.Packet.Oicq
{
    public enum OicqStatus
    {
        OK = 0,

        DoVerifySliderCaptcha = 2,

        DoVerifyDeviceLock = 204,
        DoVerifySms = 160,
        DoVerifyDeviceLockViaSmsNewArea = 239,

        PreventByIncorrectUserOrPwd = 1,
        PreventByReceiveIssue = 3,
        PreventByAccountFrozen = 40,
        PreventBySmsSentFailed = 162,
        PreventByIncorrectSmsCode = 163,
        PreventByLoginDenied = 167,
        PreventByInvalidEnvironment = 237,
    }

    public enum OicqEncryptMethod
    {
        ECDH7 = 0x07,
        ECDH135 = 0x87,
    }

    public class OicqRequest : PacketBase
    {
        public readonly uint uin;
        public readonly ushort oicqCommand;
        public readonly ushort oicqSubCommand;
        public readonly ushort oicqVersion;
        public readonly OicqStatus oicqStatus;
        public readonly OicqRequestBody oicqRequestBody;
        public readonly OicqEncryptMethod oicqEncryptMethod;

        public OicqRequest(ushort command, ushort subCommand, uint uin,
            OicqEncryptMethod method, OicqRequestBody body,
            byte[] shareKey, byte[] randKey, byte[] publicKey)
            : base()
        {
            this.uin = uin;
            oicqVersion = 8001;
            oicqCommand = command;
            oicqSubCommand = subCommand;
            oicqRequestBody = body;
            oicqEncryptMethod = method;

            PutByte(0x02); // 頭部 0x02
            {
                EnterBarrier(Prefix.Uint16, Endian.Big, 4);
                {
                    PutUshortBE(oicqVersion);
                    PutUshortBE(oicqCommand);
                    PutUshortBE(1);
                    PutUintBE(this.uin);
                    PutByte(0x03);
                    PutByte((byte)method);
                    PutByte(0x00); // 永遠0
                    PutUintBE(2);
                    PutUintBE(AppInfo.AppClientVersion);
                    PutUintBE(0);

                    PutOicqRequestBody(oicqRequestBody, shareKey, randKey, publicKey);
                }
                LeaveBarrier();
            }
            PutByte(0x03); // 尾部 0x03
        }

        public OicqRequest(byte[] data, byte[] shareKey) : base(data)
        {
            EatBytes(1);
            {
                EatBytes(2);

                TakeUshortBE(out oicqVersion);
                TakeUshortBE(out oicqCommand);
                EatBytes(2);

                TakeUintBE(out uin);

                EatBytes(2);
                TakeByte(out var status); oicqStatus = (OicqStatus)status;

                TakeOicqRequestBody(out oicqRequestBody, shareKey);
            }
            EatBytes(1);
        }

        private void TakeOicqRequestBody(out OicqRequestBody body, byte[] shareKey)
        {
            body = new OicqRequestBody(TakeBytes(out byte[] _, RemainLength - 1), shareKey);
        }

        private void PutOicqRequestBody(OicqRequestBody body, byte[] shareKey,
            byte[] randKey, byte[] publicKey)
        {
            PutUshortBE(0x0101);
            PutBytes(randKey);
            PutUshortBE(0x0102);
            PutBytes(publicKey, Prefix.Uint16);
            PutPacketEncrypted(body, TeaCryptor.Instance, shareKey);
        }
    }

    public class OicqRequestBody : PacketBase
    {
        public readonly ushort _oicqSubCommand;
        public readonly OicqStatus _oicqStatus;

        public OicqRequestBody()
            : base()
        {

        }

        public OicqRequestBody(byte[] data, byte[] shareKey)
            : base(data, TeaCryptor.Instance, shareKey)
        {
            TakeUshortBE(out _oicqSubCommand);
            _oicqStatus = (OicqStatus)TakeByte(out var _);
        }
    }

}
