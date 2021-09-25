using Konata.Core.Attributes;
using Konata.Core.Packets;
using Konata.Core.Events.Model;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.StatSvc
{
    [EventSubscribe(typeof(SimpleGetEvent))]
    [Service("StatSvc.SimpleGet", "Simple get")]
    internal class SimpleGet : BaseService<SimpleGetEvent>
    {
        protected override bool Parse(SSOFrame input,
            BotKeyStore keystore, out SimpleGetEvent output)
        {
            var root = ProtoTreeRoot.Deserialize
                (input.Payload.GetBytes(), true);
            {
                output = SimpleGetEvent.Result(
                    (int) root.GetLeafVar("08"),
                    (int) root.GetLeafVar("18"),
                    (int) root.GetLeafVar("28"),
                    root.GetLeafString("22")
                );
                return true;
            }
        }

        protected override bool Build(Sequence sequence, SimpleGetEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = input.SessionSequence;

            if (SSOFrame.Create("StatSvc.SimpleGet", PacketType.TypeB,
                newSequence, sequence.Session, new ByteBuffer(), out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    keystore.Account.Uin, keystore.Session.D2Token, keystore.Session.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, device, out output);
                }
            }

            return false;
        }
    }
}
