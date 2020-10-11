using System;
using Konata.Msf.Packets.Svc;
using Konata.Msf.Packets.Wup;

namespace Konata.Msf.Services.StatSvc
{
    internal class Register : Service
    {
        private Register()
        {
            Register("StatSvc.register", this);
        }

        public static Service Instance { get; } = new Register();

        protected override bool OnRun(Core core, string method, params object[] args)
        {
            if (method == "")
                throw new Exception("???");

            return Request_Register(core);
        }

        protected override bool OnHandle(Core core, params object[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            return Handle_Register(core);
        }

        private bool Handle_Register(Core core)
        {
            return false;
        }

        private bool Request_Register(Core core)
        {
            var requestBody = new SvcReqRegister.XSvcRegister()
            {
                _uin = core._uin,
                _bid = 7,
                _connType = 0,
                _other = "",
                _status = 11,
                _onlinePush = 0,
                _isOnline = 0,
                _isShowOnline = 0,
                _kikPC = 0,
                _kikWeak = 0,
                _timeStamp = 56,
                _osVersion = 27,
                _netType = 1,
                _regType = 0,
                _guid = DeviceInfo.Guid,
                _localeID = 2052,
                _slientPush = 0,
                _devName = DeviceInfo.System.ModelName,
                _devType = DeviceInfo.System.ModelName,
                _osVer = DeviceInfo.System.OsVersion,
                _openPush = 1,
                _largeSeq = 99,
                _oldSSOIp = 0,
                _newSSOIp = 2081292189,
                _channelNo = "",
                _cpId = 0,
                _vendorName = DeviceInfo.System.Manufacturer,
                _vendorOSName = DeviceInfo.System.OsName,
                _osIdfa = "",
                _cmd0x769Reqbody = new byte[]
                {
                    0x0A, 0x08, 0x08, 0x2E,
                    0x10, 0x9A, 0xEF, 0x9C,
                    0xFB, 0x05, 0x0A, 0x05,
                    0x08, 0x9B, 0x02, 0x10, 0x00
                },
                _isSetStatus = 0,
                _extOnlineStatus = 0,
                _batteryStatus = 0
            };

            var request = new SvcReqRegister(0, 0, 0, 0, requestBody.Encode());
            var sequence = core._ssoMan.GetNewSequence();
            core._ssoMan.PostMessage(this, request, sequence);

            return false;
        }
    }
}
