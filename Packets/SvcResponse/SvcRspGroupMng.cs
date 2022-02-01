using Konata.Core.Packets.Wup;

namespace Konata.Core.Packets.SvcResponse
{
    public class SvcRspGroupMng : UniPacket
    {
        public SvcRspGroupMng(byte[] payload)
            : base(payload, (userdata, r) =>
            {
                var p = (SvcRspGroupMng) userdata;
            })
        {
        }
    }
}
