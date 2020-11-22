using System;

namespace Konata.Core.MQ
{
    /// <summary>
    /// 消息队列初始化参数
    /// </summary>
    public class MQConfig
    {
        /// <summary>
        /// 最大同时处理消息的Task数量
        /// </summary>
        public int MaxProcessMTask { get; set; } = 5;

        /// <summary>
        /// 该消息队列限制的最大长度
        /// </summary>
        public int MaxMQLenth { get; set; } = -1;

        /// <summary>
        /// 每次消息队列读取时新消息的超时时间
        /// </summary>
        public int ReadTimeout { get; set; } = -1;
    }
}
