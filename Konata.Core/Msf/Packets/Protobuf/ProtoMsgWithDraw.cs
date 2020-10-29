using System;
using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Protobuf
{
    /// <summary>
    /// 撤回群訊息 PbMsgWithDraw
    /// </summary>

    public class ProtoMsgWithDraw : ProtoTreeRoot
    {
        public ProtoMsgWithDraw(uint group, uint msgId)
            : base()
        {
            addTree("12", (ProtoTreeRoot t) =>
            {
                t.addLeafVar("08", 1);
                t.addLeafVar("10", 0);
                t.addLeafVar("18", group);

                t.addTree("22", (ProtoTreeRoot t2) =>
                {
                    t2.addLeafVar("08", 7499);
                    t2.addLeafVar("10", msgId);
                });

                t.addTree("2A", (ProtoTreeRoot t2) =>
                {
                    t2.addLeafVar("08", 0);
                });
            });
        }
    }
}
