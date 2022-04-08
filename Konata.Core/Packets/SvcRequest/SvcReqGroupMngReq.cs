using Konata.Core.Packets.Wup;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.JceStruct.Model;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Packets.SvcRequest;

internal class SvcReqGroupMngReq : UniPacket
{
    public SvcReqGroupMngReq(uint selfUin, uint groupUin, bool dismiss)
        : base(0x03, "KQQ.ProfileService.ProfileServantObj",
            "GroupMngReq", "GroupMngReq", 0x00, 0x00, 117456266,
            (out JStruct w) =>
            {
                w = new JStruct();
                {
                    w[0] = (JNumber) (dismiss ? 9 : 2);
                    w[1] = (JNumber) selfUin;

                    var buf = new ByteBuffer();
                    {
                        if (dismiss)
                        {
                            buf.PutUintBE(groupUin);
                            buf.PutUintBE(selfUin);
                        }

                        else
                        {
                            buf.PutUintBE(selfUin);
                            buf.PutUintBE(groupUin);
                        }
                    }

                    w[2] = new JSimpleList(buf.GetBytes());
                }
            })
    {
    }
}
