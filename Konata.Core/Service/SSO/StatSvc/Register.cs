using System;
using System.Text;

using Konata.Core.Event;
using Konata.Core.Packet;
using Konata.Core.Manager;
using Konata.Core.Packet.SvcRequest;
using Konata.Core.Packet.SvcResponse;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Service.StatSvc
{
    [SSOService("StatSvc.register", "Register client")]
    public class Register : ISSOService
    {
        public bool HandleInComing(EventSsoFrame ssoFrame, out KonataEventArgs output)
        {
            var svcResponse = new SvcRspRegister(ssoFrame.Payload.GetBytes());

            output = new EventOnlineStatus
            {
                Type = svcResponse.status ?
                    EventOnlineStatus.OnlineType.Online :
                    EventOnlineStatus.OnlineType.Offline,
            };

            return true;
        }

        public bool HandleOutGoing(KonataEventArgs eventArg, out byte[] output)
        {
            output = null;

            if (eventArg is EventOnlineStatus e)
            {
                var sigManager = e.Owner.GetComponent<UserSigManager>();
                var ssoManager = e.Owner.GetComponent<SsoInfoManager>();
                var svcRequest = new SvcReqRegister(new RegisterInfo
                {
                    uin = sigManager.Uin,
                    bid = 7,
                    connType = 0,
                    other = "",
                    status = (int)e.Type,
                    onlinePush = 0,
                    isOnline = 0,
                    isShowOnline = 0,
                    kikPC = e.IsKickPC,
                    kikWeak = false,
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
                    newSSOIp = 0,
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
                });

                if (EventSsoFrame.Create("StatSvc.register", PacketType.TypeA,
                    ssoManager.NewSequence, ssoManager.Session, svcRequest, out var ssoFrame))
                {
                    if (EventServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                        sigManager.Uin, sigManager.D2Token, sigManager.D2Key, out var toService))
                    {
                        return EventServiceMessage.Build(toService, out output);
                    }
                }
            }

            return false;
        }
    }
}