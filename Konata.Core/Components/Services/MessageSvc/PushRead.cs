using Konata.Core.Packets;
using Konata.Core.Events;
using Konata.Core.Attributes;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.MessageSvc;

[Service("MessageSvc.PushReaded", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class PushRead : BaseService<ProtocolEvent>
{
}
