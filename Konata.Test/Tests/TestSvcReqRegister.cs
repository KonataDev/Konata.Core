using System;
using Konata.Msf.Packets.Svc;
using Konata.Library.JceStruct;

namespace Konata.Test.Tests
{
    public class TestSvcReqRegister : Test
    {
        public override bool Run()
        {
            var requestBody = new SvcReqRegister.XSvcRegister()
            {
                _uin = 2842875712,
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
                _guid = new byte[]
                {
                    0x1D, 0xA5, 0xA0, 0xBC,
                    0x7E, 0x76, 0x75, 0x7D,
                    0x54, 0x71, 0x3A, 0xD9,
                    0x17, 0x5E, 0xF8, 0xDA
                },
                _localeID = 2052,
                _slientPush = 0,
                _devName = "SM-G9009W",
                _devType = "SM-G9009W",
                _osVer = "8.1.0",
                _openPush = 1,
                _largeSeq = 99,
                _oldSSOIp = 0,
                _newSSOIp = 2081292189,
                _channelNo = "",
                _cpId = 0,
                _vendorName = "SAMSUNG",
                _vendorOSName = "klteduosctc-user 6.0",
                _osIdfa = "",
                _cmd0x769Reqbody = new byte[]
                {
                    0x0a, 0x08, 0x08, 0x2E,
                    0x10, 0x9A, 0xEF, 0x9C,
                    0xFB, 0x05, 0x0A, 0x05,
                    0x08, 0x9B, 0x02, 0x10, 0x00
                },
                _isSetStatus = 0,
                _extOnlineStatus = 0,
                _batteryStatus = 0
            };

            var request = new SvcReqRegister(0, 0, 0, 0, requestBody.Encode());
            Print(request.ToString());

            return true;
        }
    }
}
