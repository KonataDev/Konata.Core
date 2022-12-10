using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Packets.SvcResponse;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.StatSvc;

[Service("StatSvc.register", PacketType.TypeA, AuthFlag.D2Authentication, SequenceMode.Managed, true)]
[EventSubscribe(typeof(OnlineStatusEvent))]
internal class Register : BaseService<OnlineStatusEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out OnlineStatusEvent output)
    {
        var svcResponse = new SvcRspRegister(input.Payload.GetBytes());

        output = OnlineStatusEvent.Result(svcResponse.status
            ? OnlineStatusEvent.Type.Online
            : OnlineStatusEvent.Type.Offline);

        return true;
    }

    protected override bool Build(int sequence, OnlineStatusEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new SvcReqRegister(new RegisterInfo
        {
            uin = keystore.Account.Uin,
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
            silentPush = 0,
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

        return true;
    }
}
