using System;
using Konata.Msf.Utils.Crypt;
using Konata.Utils;

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

    public class OicqRequest : Packet
    {
        public readonly uint _uin;
        public readonly ushort _oicqCommand;
        public readonly ushort _oicqSubCommand;
        public readonly ushort _oicqVersion;
        public readonly OicqStatus _oicqStatus;
        public readonly OicqRequestBody _oicqRequestBody;

        public OicqRequest(ushort command, ushort subCommand, uint uin) : base()
        {
            _uin = uin;
            _oicqVersion = 8001;
            _oicqCommand = command;
            _oicqSubCommand = subCommand;
        }

        internal void PackRequest()
        {
            PutByte(0x02); // 頭部 0x02
            {
                EnterBarrier(2, Endian.Big, 4);
                {
                    PutUshortBE(_oicqVersion);
                    PutUshortBE(_oicqCommand);
                    PutUshortBE(1);
                    PutUintBE(_uin);
                    PutByte(0x03);
                    PutByte(0x87); // 加密方式id
                    PutByte(0x00); // 永遠0
                    PutUintBE(2);
                    PutUintBE(AppInfo.appClientVersion);
                    PutUintBE(0);

                    PutRequestBody();
                }
                LeaveBarrier();
            }
            PutByte(0x03); // 尾部 0x03
        }

        protected virtual void PutRequestBody()
        {
            // Do Nothing Here.
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
            body = new OicqRequestBody(TakeAllBytes(out byte[] _), shareKey);
        }

        private void PutOicqRequestBody(OicqRequestBody body, byte[] shareKey)
        {

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
            TakeByte(out var status); _oicqStatus = (OicqStatus)status;



        }
    }

}
