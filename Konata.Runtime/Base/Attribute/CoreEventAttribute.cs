using System;
using Konata.Runtime.Base.Event;

namespace Konata.Runtime.Base
{
    /// <summary>
    /// 核心事件特性
    /// <para>仅限核心功能类库加载有效</para>
    /// </summary>
    public class CoreEventAttribute : BaseAttribute
    {
        /// <summary>
        /// 标记核心事件类别
        /// </summary>
        public CoreEventType EventType { get; set; }

        public EventRunType EventRunType { get; set; }

        public CoreEventAttribute(CoreEventType type,
            EventRunType runtype = EventRunType.OnlySymbol, string name = "UnDefined", string des = "")
            : base(name, des)
        {
            this.EventRunType = runtype;
            this.EventType = type;
        }
    }
}
