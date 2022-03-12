using Konata.Core.Packets.Wup;

namespace Konata.Core.Packets.SvcPush;

internal class SvcPushNotify : UniPacket
{
    public int Type { get; private set; }

    public SvcPushNotify(byte[] pushMsg)
        : base(pushMsg, (userdata, r) =>
        {
            // var p = (SvcPushNotify)userdata;
            // p.Type = r["0.5"].Number.ValueInt;
        })
    {
    }
}
