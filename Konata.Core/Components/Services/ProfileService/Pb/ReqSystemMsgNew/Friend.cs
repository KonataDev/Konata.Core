using Konata.Core.Attributes;
using Konata.Core.Events;
using Konata.Core.Packets;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.ProfileService.Pb.ReqSystemMsgNew;

[Service("ProfileService.Pb.ReqSystemMsgNew.Friend", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Friend : BaseService<ProtocolEvent>
{
}
