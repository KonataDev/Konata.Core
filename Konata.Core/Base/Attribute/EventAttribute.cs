using Konata.Core.Base.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base
{
    /// <summary>
    /// 标准事件特性
    /// </summary>
    public class EventAttribute:BaseAttribute
    {
        /// <summary>
        /// 事件名
        /// </summary>
        public string EventType { get; set; }


        public EventRunType EventRunType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">事件定义名
        /// <para>该名称用于定义该事件注册关联名称</para>
        /// </param>
        /// <param name="name">事件名
        /// <para>该名称仅用来标记该事件称呼,不会被用作框架事件注册</para>
        /// </param>
        /// <param name="des"></param>
        public EventAttribute(string type,EventRunType runtype=EventRunType.OnlySymbol ,string name = "UnDefined", string des = "")
                : base(name, des)
        {
            this.EventRunType = runtype;
            this.EventType = type;
        }
    }
}
