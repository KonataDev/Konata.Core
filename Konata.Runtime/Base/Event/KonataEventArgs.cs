using System;

namespace Konata.Runtime.Base.Event
{
    /// <summary>
    /// 标准通用事件格式
    /// </summary>
    public class KonataEventArgs : EventArgs
    {
        /// <summary>
        /// 事件拥有者[实体]
        /// </summary>
        public Entity Receiver { get; set; }

        /// <summary>
        /// 事件包对应的事件名
        /// </summary>
        public string EventName { get; set; }
    }
}
