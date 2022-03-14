using Konata.Core.Packets.Wup;

namespace Konata.Core.Packets.SvcPush;

internal class SvcPushMsfForceOffline : UniPacket
{
    public string Title { get; private set; }

    public string Message { get; private set; }

    public SvcPushMsfForceOffline(byte[] pushMsg)
        : base(pushMsg, (userdata, r) =>
        {
            var p = (SvcPushMsfForceOffline) userdata;

            p.Title = r["0.4"].String.Value;
            p.Message = r["0.3"].String.Value;
        })
    {
    }
}
