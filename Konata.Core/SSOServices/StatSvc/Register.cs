using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Runtime.Base.Event;

namespace Konata.Core.SSOServices.StatSvc
{
    [SSOService("StatSvc.Register", "Register client")]
    public class Register : ISSOService
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