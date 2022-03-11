using Konata.Core.Packets.Wup;

namespace Konata.Core.Packets.SvcPush;

public class SvcPushMsfForceOffline : UniPacket
{
    public string title;
    public string message;

    public SvcPushMsfForceOffline(byte[] pushMsg)
        : base(pushMsg, (userdata, r) =>
        {
            var p = (SvcPushMsfForceOffline) userdata;

            p.title = r["0.4"].String.Value;
            p.message = r["0.3"].String.Value;
        })
    {
    }
}
