using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Runtime.Base.Event;

namespace Konata.Core.SSOServices.OidbSvc
{
    [SSOService("OidbSvc.0x8a0_1", "Kick member in the group")]
    class Oidb0x8a0_1 : ISSOService
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
