using System;
using Konata.Msf.Packets.Wup;
using Konata.Library.JceStruct;

namespace Konata.Msf.Packets.Svc
{
    public class SvcReqEmpty : UniPacket
    {
        public SvcReqEmpty()
            : base("PushService", "SvcReqEmpty", 0, 0, 0, 0,
                  new XSvcEmpty())
        {

        }

        public class XSvcEmpty : UniPacketBodyV3
        {
            public XSvcEmpty()
                : base("SvcReqEmpty", new Func<JceTreeRoot>(() =>
                {
                    return new JceTreeRoot()
                    .AddStruct(0, (JceTreeRoot leaf) =>
                    {
                        leaf.AddLeafString(0, "This is an empty request.");
                    });

                })())
            {

            }
        }
    }
}
