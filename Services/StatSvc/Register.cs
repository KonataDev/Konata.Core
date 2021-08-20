using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Packets.SvcResponse;
using Konata.Core.Attributes;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.StatSvc
{
    [Service("StatSvc.register", "Register client")]
    [EventSubscribe(typeof(OnlineStatusEvent))]
    public class Register : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signinfo, out ProtocolEvent output)
        {
            var svcResponse = new SvcRspRegister(input.Payload.GetBytes());

            output = OnlineStatusEvent.Result(svcResponse.status
                ? OnlineStatusEvent.Type.Online
                : OnlineStatusEvent.Type.Offline);

            return true;
        }

        public bool Build(Sequence sequence, OnlineStatusEvent input,
            BotKeyStore signinfo, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var svcRequest = new SvcReqRegister(new RegisterInfo
            {
                uin = signinfo.Account.Uin,
                bid = 7,
                connType = 0,
                other = "",
                status = (int) input.EventType,
                onlinePush = 0,
                isOnline = 0,
                isShowOnline = 0,
                kikPC = input.IsKickPC,
                kikWeak = false,
                timeStamp = 56,
                osVersion = 27,
                netType = 1,
                regType = 0,
                guid = device.System.Guid,
                localeID = 2052,
                slientPush = 0,
                devName = device.Model.Name,
                devType = device.Model.Name,
                osVer = device.System.Version,
                openPush = 1,
                largeSeq = 99,
                oldSSOIp = 0,
                newSSOIp = 0,
                channelNo = "",
                cpId = 0,
                vendorName = device.Model.Manufacturer,
                vendorOSName = device.System.Name,
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
                signinfo.Session.TgtToken, sequence.Session, svcRequest, out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    signinfo.Account.Uin, signinfo.Session.D2Token, signinfo.Session.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, device, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signinfo, BotDevice device, out int outsequence, out byte[] output)
            => Build(sequence, (OnlineStatusEvent) input, signinfo, device, out outsequence, out output);
    }
}
