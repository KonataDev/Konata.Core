using Konata.Core.Packets.Wup;

namespace Konata.Core.Packets.SvcPush;

internal class SvcPushForceOffline : UniPacket
{
    public string Title { get; private set; }

    public string Message { get; private set; }

    public SvcPushForceOffline(byte[] pushMsg)
        : base(pushMsg, (userdata, r) =>
        {
            var p = (SvcPushForceOffline) userdata;

            p.Title = r["0.1"].String.Value;
            p.Message = r["0.2"].String.Value;
        })
    {
    }
}
