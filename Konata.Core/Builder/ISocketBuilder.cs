using Konata.Core.NetWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Builder
{
    public interface ISocketBuilder
    {
        IDictionary<string, object> Properties { get; }

        Type SocketType { get; }

        IDictionary<string,object> Sources { get; }
        ISocketBuilder SetCustomSocket(Type socket);
        ISocket Build();
    }
}
