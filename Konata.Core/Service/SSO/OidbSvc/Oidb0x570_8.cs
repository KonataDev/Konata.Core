﻿using System;
using System.Text;

using Konata.Core.Event;
using Konata.Core.Packet;
using Konata.Core.Manager;
using Konata.Core.Packet.Oidb.OidbModel;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Service.OidbSvc
{
    [SSOService("OidbSvc.0x570_8", "Mute member in the group")]
    public class Oidb0x570_8 : ISSOService
    {
        public bool HandleInComing(EventSsoFrame ssoMessage, out KonataEventArgs output)
        {
            throw new NotImplementedException();
        }

        public bool HandleOutGoing(KonataEventArgs eventArg, out byte[] output)
        {
            output = null;

            if (eventArg is EventGroupMuteMember e)
            {
                var sigManager = e.Owner.GetComponent<UserSigManager>();
                var ssoManager = e.Owner.GetComponent<SsoInfoManager>();
                var oidbRequest = new OidbCmd0x570_8(e.GroupUin, e.MemberUin, e.TimeSeconds ?? uint.MaxValue);

                if (EventSsoFrame.Create("OidbSvc.0x570_8", PacketType.TypeB,
                    ssoManager.NewSequence, ssoManager.Session, oidbRequest, out var ssoFrame))
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