using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Runtime.Base.Event;

namespace Konata.Core.SSOServices.MessageSvc
{
    [SSOService("MessageSvc.PushNotify", "Push notify on received a private message")]
    public class PushNotify : ISSOService
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
