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
        private static string RecvLenCalcer = "recvlencal";

        private static string SocketConfigkey = "SocketConfig";

        /// <summary>
        /// 设置Socket接收消息处理者
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static ISocketBuilder SetServerDataReceiver(this ISocketBuilder builder,Action<Byte[]> handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Properties[ServerDataReceiverkey] = handler;
            return builder;
        }

        /// <summary>
        /// 设置socket关闭处理者
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static ISocketBuilder SetServerCloseWatcher(this ISocketBuilder builder, Action handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Properties[ServerClosekey] = handler;
            return builder;
        }
        /// <summary>
        /// 设置包长度计算处理者
        /// 在无法计算时或者数据异常时返回-1
        /// [处于接收锁操作中,请勿执行过长时间方法]
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static ISocketBuilder SetRecvLenCalcer(this ISocketBuilder builder, Func<List<Byte>, int> handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Properties[RecvLenCalcer] = handler;
            return builder;
        }

        /// <summary>
        /// 设置初始socket通信包
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static ISocketBuilder SocketConfig(this ISocketBuilder builder,Action<SocketConfig> instance)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Sources[SocketConfigkey] = instance;
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
        public static Func<List<Byte>, int> GetRecvLenCaler(this ISocketBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (builder.Properties.TryGetValue(RecvLenCalcer, out object handler))
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
                return config;
            }
            return null;
        }
    }
}
