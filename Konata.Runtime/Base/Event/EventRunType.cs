using System;

namespace Konata.Runtime.Base.Event
{
    /// <summary>
    /// 事件模式
    /// </summary>
    public enum EventRunType
    {
        /// <summary>
        /// 仅标识
        /// </summary>
        OnlySymbol = 0,
        /// <summary>
        /// 在调度监听者前处理
        /// </summary>
        BeforeListener = 1,
        /// <summary>
        /// 在调度监听者后处理
        /// </summary>
        AfterListener = 2,
    }
}
