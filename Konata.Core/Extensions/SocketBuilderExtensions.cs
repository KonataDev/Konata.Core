using Konata.Core.Builder;
using Konata.Core.NetWork;
using System;
using System.Collections.Generic;

namespace Konata.Core.Extensions
{
    public static class SocketBuilderExtensions
    {

        private static string ServerDataReceiverkey = "ServerDataReceiver";
        private static string ServerClosekey = "ServerCloseWatcher";
        private static string CheckContinuekey = "CheckContinueReceive";
        private static string recvdatalenkey = "recvdatalen";

        private static string SocketConfigkey = "SocketConfig";


        public static ISocketBuilder SetServerDataReceiver(this ISocketBuilder builder,Action<Byte[]> handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Properties[ServerDataReceiverkey] = handler;
            return builder;
        }

        public static ISocketBuilder SetServerCloseWatcher(this ISocketBuilder builder, Action handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Properties[ServerClosekey] = handler;
            return builder;
        }

        public static ISocketBuilder SetContinueReceiveChecker(this ISocketBuilder builder,Func<List<Byte>, bool> handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Properties[CheckContinuekey] = handler;
            return builder;
        }
        public static ISocketBuilder SetrecvdatalenCounter(this ISocketBuilder builder, Func<List<Byte>, int> handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Properties[recvdatalenkey] = handler;
            return builder;
        }

        public static ISocketBuilder SocketConfig(this ISocketBuilder builder,Action<SocketConfig> instance)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Properties[SocketConfigkey] = instance;
            return builder;
        }
        public static Action<Byte[]> GetServerDataReceiver(this ISocketBuilder builder)
        {
            if(builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if(builder.Properties.TryGetValue(ServerDataReceiverkey,out object handler))
            {
                return handler as Action<Byte[]>;
            }
            return null;
        }
        public static Action GetServerCloseListener(this ISocketBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (builder.Properties.TryGetValue(ServerClosekey, out object handler))
            {
                return handler as Action;
            }
            return null;
        }
        public static Func<List<Byte>, bool> GetContinueReceiveChecker(this ISocketBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (builder.Properties.TryGetValue(CheckContinuekey, out object handler))
            {
                return handler as Func<List<Byte>, bool>;
            }
            return null;
        }
        public static Func<List<Byte>, int> GetrecvdatalenCounter(this ISocketBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (builder.Properties.TryGetValue(CheckContinuekey, out object handler))
            {
                return handler as Func<List<Byte>, int>;
            }
            return null;
        }

        public static SocketConfig GetSocketConfig(this ISocketBuilder builder)
        {
            SocketConfig config = new SocketConfig();
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (builder.Sources.TryGetValue(SocketConfigkey, out object instance))
            {
                    (instance as Action<SocketConfig>).Invoke(config);
            }
            return null;
        }
    }
}
