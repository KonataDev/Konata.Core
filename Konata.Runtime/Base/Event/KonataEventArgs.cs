using System;

namespace Konata.Runtime.Base.Event
{
    /// <summary>
    /// 标准通用事件格式
    /// </summary>
    public class KonataEventArgs : EventArgs
    {
        public object Sender { get; set; }

        public Entity Receiver { get; set; }

        /// <summary>
        /// 负载
        /// </summary>
        public object Data { get; set; }
    }
}
