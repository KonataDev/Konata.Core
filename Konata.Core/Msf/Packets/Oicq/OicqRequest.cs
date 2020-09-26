using System;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Oicq
{
    public enum OicqStatus
    {
        OK = 0,
        DoVerifySlider = 2,

        DoVerifyDeviceLock = 204,
        DoVerifyDeviceLockViaSms = 160,
        DoVerifyDeviceLockViaSmsNewArea = 239,

        PreventByIncorrectUserOrPwd = 1,
        PreventByReceiveIssue = 3,
        PreventByAccountFrozen = 40,
        PreventBySmsSentFailed = 162,
        PreventByLoginArea = 237,
    }

    public enum OicqEncryptMethod
    {
        ECDH7 = 0x07,
        ECDH135 = 0x87,
    }

    public class OicqRequest : Packet
    {
        public readonly uint _uin;
        public readonly ushort _oicqCommand;
        public readonly ushort _oicqSubCommand;
        public readonly ushort _oicqVersion;
        public readonly OicqStatus _oicqStatus;
        public readonly OicqRequestBody _oicqRequestBody;
        public readonly OicqEncryptMethod _oicqEncryptMethod;

        public OicqRequest(ushort command, ushort subCommand, uint uin,
            OicqEncryptMethod method, OicqRequestBody body,
            byte[] shareKey, byte[] randKey, byte[] publicKey)
            : base()
        {
            _uin = uin;
            _oicqVersion = 8001;
            _oicqCommand = command;
            _oicqSubCommand = subCommand;
            _oicqRequestBody = body;
            _oicqEncryptMethod = method;

            PutByte(0x02); // 頭部 0x02
            {
                EnterBarrier(2, Endian.Big, 4);
                {
                    PutUshortBE(_oicqVersion);
                    PutUshortBE(_oicqCommand);
                    PutUshortBE(1);
                    PutUintBE(_uin);
                    PutByte(0x03);
                    PutByte((byte)method);
                    PutByte(0x00); // 永遠0
                    PutUintBE(2);
                    PutUintBE(AppInfo.appClientVersion);
                    PutUintBE(0);

                    PutOicqRequestBody(_oicqRequestBody, shareKey, randKey, publicKey);
                }
                LeaveBarrier();
            }
            PutByte(0x03); // 尾部 0x03
        }

        public OicqRequest(byte[] data, byte[] shareKey) : base(data)
        {
            EatBytes(4);

            EatBytes(1);
            {
                EatBytes(2);

                TakeUshortBE(out _oicqVersion);
                TakeUshortBE(out _oicqCommand);
                EatBytes(2);

                TakeUintBE(out _uin);

                EatBytes(2);
                TakeByte(out var status); _oicqStatus = (OicqStatus)status;

                TakeOicqRequestBody(out _oicqRequestBody, shareKey);
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
            PutBytes(publicKey, 2);
            PutPacketEncrypted(body, TeaCryptor.Instance, shareKey);
        }
    }

    public class OicqRequestBody : Packet
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
