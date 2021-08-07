using System;
using System.Collections.Generic;

using Konata.Core.Message.Model;

namespace Konata.Core.Events.Model
{
    public class PrivateOffPicUpEvent : ProtocolEvent
    {
        public List<ImageChain> Images { get; set; }
    }
}
