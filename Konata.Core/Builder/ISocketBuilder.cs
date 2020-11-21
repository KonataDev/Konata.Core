using System;
using System.Collections.Generic;

using Konata.Core.NetWork;

namespace Konata.Core.Builder
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
