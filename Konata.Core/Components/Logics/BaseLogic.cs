using System;
using System.Threading.Tasks;
using Konata.Core.Events;

namespace Konata.Core.Components.Logics;

internal class BaseLogic : IBaseLogic
{
    internal BusinessComponent Context { get; }

    internal SocketComponent SocketComponent
        => Context.GetComponent<SocketComponent>();

    internal BaseLogic(BusinessComponent context)
    {
        Context = context;
    }

    public virtual Task Incoming(ProtocolEvent e)
        => throw new NotImplementedException();
}
