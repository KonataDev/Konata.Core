using System;
using System.Text;

using Konata.Core.Packet;
using Konata.Runtime.Base.Event;

namespace Konata.Core.SSOServices.PbMessageSvc
{
    [SSOService("PbMessageSvc.PbMsgReadedReport", "Push have been read message")]
    public class PbMsgReadReport : ISSOService
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
