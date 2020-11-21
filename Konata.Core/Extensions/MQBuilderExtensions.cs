using Konata.Core.Builder;
using Konata.Core.MQ;
using Konata.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Extensions
{
    public static class MQBuilderExtensions
    {
        private static string MQReceiverkey = "MQReceiver";
        private static string MQexternalTaskQueuekey = "MQexternalTaskQueue";


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
        /// 使用外部TaskQueue进行读取线程管理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IMQBuilder<T> SetExternalTaskQueue<T>(this IMQBuilder<T> builder,TaskQueue instance)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Sources[MQexternalTaskQueuekey] = instance;
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

        public static List<Action<T>> GetMQReceiver<T>(this IMQBuilder<T> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (builder.Properties.TryGetValue(MQReceiverkey, out object handlers))
            {
                return handlers as List<Action<T>>;
            }
            return null;
        }
        public static TaskQueue GetExternalTaskQueue<T>(this IMQBuilder<T> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (builder.Properties.TryGetValue(MQexternalTaskQueuekey, out object instance))
            {
                return instance as TaskQueue;
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
