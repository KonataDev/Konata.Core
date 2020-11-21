using Konata.Core.MQ;
using System;
using System.Collections.Generic;

namespace Konata.Core.Builder
{
    public class MQBuilder<T> : IMQBuilder<T>
    {
        private static Type stype = typeof(IMQ<T>);
        private IDictionary<string, object> properties = new Dictionary<string, object>();

        public IDictionary<string, object> Properties
        {
            get => properties;
        }

        private IDictionary<string, object> sources = new Dictionary<string, object>();
        public IDictionary<string, object> Sources
        {
            get => sources;
        }

        private Type type = typeof(KonataMemMQ<T>);
        public Type MQType
        {
            get => type;
        }

        public IMQ<T> Build()
        {
            return (IMQ<T>)Activator.CreateInstance(MQType, new object[] { this });
        }

        /// <summary>
        /// 使用自定义MQ用例
        /// 需要实现IMQ<T>
        /// </summary>
        /// <param name="mq"></param>
        /// <returns></returns>
        public IMQBuilder<T> SetCustomMQ(Type mq)
        {
            if (stype.IsAssignableFrom(mq))
            {
                this.type = mq;
                return this;
            }
            else
            {
                throw new ArgumentException(nameof(mq));
            }
        }
    }
}
