using System;
using Konata.Msf.Packets.SvcReq;

namespace Konata.Msf.Services.StatSvc
{
    public class Register : Service
    {
        private Register()
        {
            Register("StatSvc.register", this);
        }

        public static Service Instance { get; } = new Register();

        public override bool OnRun(Core core, string method, params object[] args)
        {
            if (method != "")
                throw new Exception("???");

            return Request_Register(core);
        }

        public override bool OnHandle(Core core, params object[] args)
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
            var info = new RegisterInfo()
            {
                uin = core.SigInfo.Uin,
                bid = 7,
                connType = 0,
                other = "",
                status = 11,
                onlinePush = 0,
                isOnline = 0,
                isShowOnline = 0,
                kikPC = 0,
                kikWeak = 0,
                timeStamp = 56,
                osVersion = 27,
                netType = 1,
                regType = 0,
                guid = DeviceInfo.Guid,
                localeID = 2052,
                slientPush = 0,
                devName = DeviceInfo.System.ModelName,
                devType = DeviceInfo.System.ModelName,
                osVer = DeviceInfo.System.OsVersion,
                openPush = 1,
                largeSeq = 99,
                oldSSOIp = 0,
                newSSOIp = 2081292189,
                channelNo = "",
                cpId = 0,
                vendorName = DeviceInfo.System.Manufacturer,
                vendorOSName = DeviceInfo.System.OsName,
                osIdfa = "",
                cmd0x769Reqbody = new byte[]
                {
                    0x0A, 0x08, 0x08, 0x2E,
                    0x10, 0x9A, 0xEF, 0x9C,
                    0xFB, 0x05, 0x0A, 0x05,
                    0x08, 0x9B, 0x02, 0x10, 0x00
                },
                isSetStatus = 0,
                extOnlineStatus = 0,
                batteryStatus = 0
            };

            var request = new SvcReqRegister(0, 0, 0, 0, info);
            var sequence = core.SsoMan.GetNewSequence();
            core.SsoMan.PostMessage(this, request, sequence);

            return true;
        }
    }
}
