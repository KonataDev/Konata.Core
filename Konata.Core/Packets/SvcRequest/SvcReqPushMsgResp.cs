using System.Collections.Generic;
using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

namespace Konata.Core.Packets.SvcRequest;

public class SvcReqPushMsgResp : UniPacket
{
    public SvcReqPushMsgResp(int reqid, uint selfUin, uint source, int v1, int svrip, byte[] v8, int v32)
        : base(0x03, "OnlinePush", "SvcRespPushMsg", "resp", 0x00, 0x00, reqid,
            (out JStruct w) =>
            {
                w = new JStruct();
                {
                    w[0] = (JNumber) selfUin; // uin
                    w[1] = new JList();
                    if (source != 0)
                    {
                        ((JList) w[1]).Add(new JStruct
                        {
                            [0] = (JNumber) source,
                            [1] = (JNumber) v1,
                            [2] = (JNumber) v32,
                            [3] = (JSimpleList) v8,
                            [4] = (JNumber) 0,
                            [5] = (JNumber) 0,
                            [6] = (JNumber) 0,
                            [7] = (JNumber) 0,
                            [8] = (JNumber) 0,
                            [9] = (JNumber) 0,
                            [10] = (JNumber) 0
                        });
                    }
                }

                w[2] = (JNumber) svrip;
                w[4] = (JNumber) 0;
            })

    {
    }
}
