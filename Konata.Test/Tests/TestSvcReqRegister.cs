using System;
using Konata.Msf.Packets.SvcRequest;

namespace Konata.Test.Tests
{
    public class TestSvcReqRegister : Test
    {
        public override bool Run()
        {
            var info = new RegisterInfo()
            {
                uin = 2842875712,
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
                guid = new byte[]
                {
                    0x1D, 0xA5, 0xA0, 0xBC,
                    0x7E, 0x76, 0x75, 0x7D,
                    0x54, 0x71, 0x3A, 0xD9,
                    0x17, 0x5E, 0xF8, 0xDA
                },
                localeID = 2052,
                slientPush = 0,
                devName = "SM-G9009W",
                devType = "SM-G9009W",
                osVer = "8.1.0",
                openPush = 1,
                largeSeq = 99,
                oldSSOIp = 0,
                newSSOIp = 2081292189,
                channelNo = "",
                cpId = 0,
                vendorName = "SAMSUNG",
                vendorOSName = "klteduosctc-user 6.0",
                osIdfa = "",
                cmd0x769Reqbody = new byte[]
                {
                    0x0a, 0x08, 0x08, 0x2E,
                    0x10, 0x9A, 0xEF, 0x9C,
                    0xFB, 0x05, 0x0A, 0x05,
                    0x08, 0x9B, 0x02, 0x10, 0x00
                },
                isSetStatus = 0,
                extOnlineStatus = 0,
                batteryStatus = 0
            };

            var request = new SvcReqRegister(info);
            Print(request.ToString());

            return true;
        }
    }
}
