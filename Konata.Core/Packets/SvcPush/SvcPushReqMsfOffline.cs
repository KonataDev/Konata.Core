using Konata.Core.Packets.Wup;

namespace Konata.Core.Packets.SvcPush;

internal class SvcPushReqMsfOffline : UniPacket
{
    public string Title { get; private set; }

    public string Message { get; private set; }

    public SvcPushReqMsfOffline(byte[] pushMsg)
        : base(pushMsg, (userdata, r) =>
        {
            var p = (SvcPushReqMsfOffline) userdata;

            p.Title = r["0.4"].String.Value;
            p.Message = r["0.3"].String.Value;
        })
    {
    }
}
