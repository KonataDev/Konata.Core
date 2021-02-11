using System;

using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Packet;
using Konata.Core.Packet.Oidb.OidbModel;

namespace Konata.Core.Service.OidbSvc
{
    [Service("OidbSvc.0x8fc_2", "Set special title for member")]
    [EventDepends(typeof(GroupSpecialTitleEvent))]
    public class Oidb0x8fc_2 : IService
    {
        public bool Parse(SSOFrame input, SignInfo signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, GroupSpecialTitleEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var oidbRequest = new OidbCmd0x8fc_2(input.GroupUin, input.MemberUin, input.SpecialTitle,
                input.TimeSeconds ?? uint.MaxValue);

            if (SSOFrame.Create("OidbSvc.0x8fc_2", PacketType.TypeB,
                newSequence, sequence.Session, oidbRequest, out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    signInfo.UinInfo.Uin, signInfo.D2Token, signInfo.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
            => Build(sequence, (GroupSpecialTitleEvent)input, signInfo, out newSequence, out output);
    }
}
