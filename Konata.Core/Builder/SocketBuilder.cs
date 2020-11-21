using System;
using System.Collections.Generic;

using Konata.Core.NetWork;

namespace Konata.Core.Builder
{
    public class SocketBuilder : ISocketBuilder
    {
        private static Type stype = typeof(ISocket);
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

        private Type type = typeof(KonataSocket);

        public Type SocketType
        {
            get => type;
        }

        public ISocket Build()
        {
            return (ISocket)Activator.CreateInstance(SocketType, new object[] { this });
        }

        /// <summary>
        /// 使用自定义Socket用例
        /// 需要实现ISocket
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public ISocketBuilder SetCustomSocket(Type socket)
        {
            if (stype.IsAssignableFrom(socket))
            {
                this.type = socket;
                return this;
            }
            else
            {
                throw new ArgumentException(nameof(socket));
            }
        }
    }
}
