using System;
using Konata.Utils.Jce;

namespace Konata.Msf.Packets.Svc
{
    public class SvcReqEmpty : SvcReq
    {
        public SvcReqEmpty()
            : base("PushService", "SvcReqEmpty", 0, 0, 0, 0,
                  new XSvcEmpty())
        {

        }

        public class XSvcEmpty : SvcReqBody
        {
            public XSvcEmpty()
                : base()
            {
                PutJceTypeHeader(JceType.StructBegin, 0);
                {
                    Write("This is an empty request.", 0);
                }
                PutJceTypeHeader(JceType.StructEnd, 0);
            }
        }
    }
}
