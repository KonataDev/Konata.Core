using System;
using System.Collections.Generic;

using Konata.Runtime.MQ;

namespace Konata.Runtime.Builder
{
    public interface IMQBuilder<T>
    {
        Type MQType { get; }

        IDictionary<string, object> Sources { get; }

        IDictionary<string, object> Properties { get; }

        IMQ<T> Build();

        IMQBuilder<T> SetCustomMQ(Type mq);
    }
}
