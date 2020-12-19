using System;

namespace Konata.Runtime.Base.Event
{
    /// <summary>
    /// 核心事件类型表
    /// 默认事件类型
    /// </summary>
    public enum CoreEventType
    {
        /// <summary>
        /// 未定义事件[保留位]
        /// </summary>
        UnDefined = 0,
        /// <summary>
        /// 自定义，此时以Eventname为主
        /// </summary>
        Custom=1,
        /// <summary>
        /// Task完成通知
        /// </summary>
        TaskComplate=2,
    }
}
