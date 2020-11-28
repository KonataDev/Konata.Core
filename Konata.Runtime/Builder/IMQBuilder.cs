using System;
using System.Collections.Generic;

using Konata.Runtime.MQ;

namespace Konata.Runtime.Builder
{
    public interface IMQBuilder<T>
    {
        IDictionary<string, object> Properties { get; }

        Type MQType { get; }

        IDictionary<string, object> Sources { get; }
        IMQBuilder<T> SetCustomMQ(Type mq);
        IMQ<T> Build();
    }
}
