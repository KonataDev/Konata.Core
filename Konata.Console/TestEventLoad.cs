using Konata.Core.Base;
using Konata.Core.Base.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Console
{
    [Event(type: "DataComing", runtype: EventRunType.BeforeListener,
        name: "数据到来事件", des: "当数据到来时候触发的事件")]
    public class Test1 : IEvent
    {
        public void Handle(KonataEventArgs arg)
        {
            System.Console.WriteLine("active!");
        }
    }

    [Event(type: "OnDataFixed", runtype: EventRunType.OnlySymbol,
    name: "数据处理完毕时", des: "当数据被处理完毕时的执行内容")]
    public class Test2
    {
    }
}
