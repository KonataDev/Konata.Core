using Konata.Core.Events.Model;
using Konata.Core.Packets.Wup;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Packets.SvcPush;

internal class SvcPushSharpVideoMsg : UniPacket
{
    public SharpSvrEvent.CallStatus Status { get; private set; }

    public byte[] PbBody { get; private set; }

    public SvcPushSharpVideoMsg(byte[] pushMsg)
        : base(pushMsg, (userdata, r) =>
        {
            var p = (SvcPushSharpVideoMsg) userdata;

            p.Status = (SharpSvrEvent.CallStatus) r["0.7"].Number.ValueInt;
            p.PbBody = r["0.4"].SimpleList.Value;
        })
    {
    }
}
