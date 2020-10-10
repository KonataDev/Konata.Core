using System;
using Konata.Utils.Jce;
using Konata.Msf.Packets.Svc;

namespace Konata.Test.Tests
{
    public class TestSvcReqRegister : Test
    {
        public override bool Run()
        {
            var request = new SvcReqRegister();
            request._uin = 3322047216;
            request._bid = 7;
            request._connType = 0;
            request._other = "";
            request._status = 11;
            request._onlinePush = 0;
            request._isOnline = 0;
            request._isShowOnline = 0;
            request._kikPC = 0;
            request._kikWeak = 0;
            request._timeStamp = 56;
            request._osVersion = 27;
            request._netType = 1;
            request._regType = 0;
            request._guid = new byte[]
            {
                0x1D, 0xA5, 0xA0, 0xBC,
                0x7E, 0x76, 0x75, 0x7D,
                0x54, 0x71, 0x3A, 0xD9,
                0x17, 0x5E, 0xF8, 0xDA
            };
            request._localeID = 2052;
            request._slientPush = 0;
            request._devName = "SM-G9009W";
            request._devType = "SM-G9009W";
            request._osVer = "8.1.0";
            request._openPush = 1;
            request._largeSeq = 99;
            request._oldSSOIp = 0;
            request._newSSOIp = 2081292189;
            request._channelNo = "";
            request._cpId = 0;
            request._vendorName = "SAMSUNG";
            request._vendorOSName = "klteduosctc-user 6.0";
            request._osIdfa = "";
            request._cmd0x769Reqbody = new byte[] {
                0x0a, 0x08, 0x08, 0x2E,
                0x10, 0x9A, 0xEF, 0x9C,
                0xFB, 0x05, 0x0A, 0x05,
                0x08, 0x9B, 0x02, 0x10, 0x00
            };
            request._isSetStatus = 0;
            request._extOnlineStatus = 0;
            request._batteryStatus = 0;

            request.WriteTo();
            Print(request.ToString());

            return true;
        }
    }
}
