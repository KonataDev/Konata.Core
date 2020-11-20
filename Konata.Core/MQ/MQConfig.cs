using System;
using System.Collections.Generic;
using System.Text;

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

    }
}
