using Konata.Core.Builder;
using Konata.Core.MQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Extensions
{
    public static class MQBuilderExtensions
    {
        private static string MQReceiverkey = "MQReceiver";
        private static string ServerClosekey = "ServerCloseWatcher";
        private static string CheckContinuekey = "CheckContinueReceive";
        private static string RecvLenCalcer = "recvlencal";

        private static string MQConfigkey = "MQConfig";

        /// <summary>
        /// 添加新的消息队列接收处理
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IMQBuilder<T> AddMQReceiver<T>(this IMQBuilder<T> builder, Action<T> handler)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if(builder.Properties.TryGetValue(MQReceiverkey, out object receiverlist))
            {
                (receiverlist as List<Action<T>>).Add(handler);
            }
            else
            {
                List<Action<T>> actions = new List<Action<T>>();
                actions.Add(handler);
                builder.Properties[MQReceiverkey] = actions;
            }
            
            return builder;
        }

        /// <summary>
        /// 设置MQ配置信息
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IMQBuilder<T> MQConfig<T>(this IMQBuilder<T> builder, Action<MQConfig> instance)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Sources[MQConfigkey] = instance;
            return builder;
        }

        public static List<Action<T>> GetServerDataReceiver<T>(this IMQBuilder<T> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (builder.Properties.TryGetValue(MQReceiver, out object handlers))
            {
                return handlers as List<Action<T>>;
            }
            return null;
        }

        public static MQConfig GetMQConfig<T>(this IMQBuilder<T> builder)
        {
            MQConfig config = new MQConfig();
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (builder.Sources.TryGetValue(MQConfigkey, out object instance))
            {
                (instance as Action<MQConfig>).Invoke(config);
                return config;
            }
            return null;
        }
    }
}
