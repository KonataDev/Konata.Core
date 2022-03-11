using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

namespace Konata.Core.Packets.SvcRequest;

internal class SvcReqHttpServerListReq : UniPacket
{
    public SvcReqHttpServerListReq()
        : base(0x03, "ConfigHttp", "HttpServerListReq", "HttpServerListReq", 0x00, 0x00, 0,
            (out JStruct w) => w = new JStruct
            {
                [1] = (JNumber) 0,
                [2] = (JNumber) 0,
                [3] = (JNumber) 1,
                [4] = (JString) "00000",
                [5] = (JNumber) 100,
                [6] = (JNumber) AppInfo.SubAppId,
                [7] = (JString) "356235088634151",
                [8] = (JNumber) 0,
                [9] = (JNumber) 0,
                [10] = (JNumber) 0,
                [11] = (JNumber) 0,
                [12] = (JNumber) 0,
                [13] = (JNumber) 0,
                [14] = (JNumber) 1
            })
    {
    }
}
