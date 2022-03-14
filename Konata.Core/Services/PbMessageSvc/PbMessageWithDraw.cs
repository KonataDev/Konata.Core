using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Attributes;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.PbMessageSvc;

[Service("PbMessageSvc.PbMessageWithDraw", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PbMessageWithDraw : BaseService<ProtocolEvent>
{
}
