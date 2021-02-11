using System;

using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Packet;
using Konata.Core.Packet.SvcRequest;
using Konata.Core.Packet.SvcResponse;

namespace Konata.Core.Service.StatSvc
{
    [Service("StatSvc.register", "Register client")]
    [EventDepends(typeof(OnlineStatusEvent))]
    public class Register : IService
    {
        public bool Parse(SSOFrame input, SignInfo signinfo, out ProtocolEvent output)
        {
            var svcResponse = new SvcRspRegister(input.Payload.GetBytes());

            output = new OnlineStatusEvent
            {
                EventType = svcResponse.status ?
                    OnlineStatusEvent.Type.Online : OnlineStatusEvent.Type.Offline,
            };

            return true;
        }

        public bool Build(Sequence sequence, OnlineStatusEvent input, SignInfo signinfo,
            out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var svcRequest = new SvcReqRegister(new RegisterInfo
            {
                uin = signinfo.UinInfo.Uin,
                bid = 7,
                connType = 0,
                other = "",
                status = (int)input.EventType,
                onlinePush = 0,
                isOnline = 0,
                isShowOnline = 0,
                kikPC = input.IsKickPC,
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

            if (SSOFrame.Create("StatSvc.register", PacketType.TypeA, newSequence,
                 signinfo.TgtToken, sequence.Session, svcRequest, out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    signinfo.UinInfo.Uin, signinfo.D2Token, signinfo.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input, SignInfo signinfo,
            out int outsequence, out byte[] output)
            => Build(sequence, (OnlineStatusEvent)input, signinfo, out outsequence, out output);
    }
}