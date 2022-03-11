using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcPush;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.StatSvc
{
    [EventSubscribe(typeof(ReqMSFOfflineEvent))]
    [Service("StatSvc.ReqMSFOffline", "Request MSF force offline.")]
    internal class ReqMSFOffline : BaseService<ReqMSFOfflineEvent>
    {
        protected override bool Parse(SSOFrame input,
            BotKeyStore keystore, out ReqMSFOfflineEvent output)
        {
            var tree = new SvcPushMsfForceOffline(input.Payload.GetBytes());
            output = new ReqMSFOfflineEvent(tree.title, tree.message);
            return true;
        }
    }
}
