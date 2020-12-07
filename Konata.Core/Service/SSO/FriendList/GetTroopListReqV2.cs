﻿using System;
using System.Text;
using Konata.Core.Event;
using Konata.Core.Packet;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Service.Friendlist
{
    [SSOService("friendlist.GetTroopListReqV2", "Pull group list")]
    public class GetTroopListReqV2 : ISSOService
    {
        public bool HandleInComing(EventSsoFrame ssoMessage, out KonataEventArgs output)
        {
            throw new NotImplementedException();
        }

        public bool HandleOutGoing(KonataEventArgs original, out byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}