using Konata.Core.MQ;
using System;
using System.Collections.Generic;

namespace Konata.Core.Builder
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
