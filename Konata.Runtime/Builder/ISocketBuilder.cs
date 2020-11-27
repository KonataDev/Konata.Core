using System;
using System.Collections.Generic;

using Konata.Runtime.NetWork;

namespace Konata.Runtime.Builder
{
    public interface ISocketBuilder
    {
        Type SocketType { get; }

        IDictionary<string, object> Sources { get; }

        IDictionary<string, object> Properties { get; }

        ISocket Build();

        ISocketBuilder SetCustomSocket(Type socket);
    }
}
