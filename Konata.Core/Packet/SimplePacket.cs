using Konata.Runtime.Base.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Packet
{
    [Packet("wtlogin","登陆相关")]
    public class SimplePacket : IPacketWorker
    {
        public bool DeSerialize(KonataEventArgs original, out KonataEventArgs evnentpackage)
        {
            throw new NotImplementedException();
        }

        public bool Serialize(KonataEventArgs original, out byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
