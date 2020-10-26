using System;
using Konata.Library.JceStruct;

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
                AddStruct(0, (JceTreeRoot leaf) =>
                {
                    leaf.AddLeafString(0, "This is an empty request.");
                });
            }
        }
    }
}
