using Konata.Core.Packets.Wup;
using Konata.Core.Utils.JceStruct.Model;

namespace Konata.Core.Packets.SvcResponse
{
    public class SvcRspGroupMng : UniPacket
    {
        public int Result { get; private set; }

        public SvcRspGroupMng(byte[] payload)
            : base(payload, (userdata, r) =>
            {
                var p = (SvcRspGroupMng) userdata;
                
                // Read fields
                p.Result = (int) (JNumber) r["0.1"];
            })
        {
        }
    }
}
