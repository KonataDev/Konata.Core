using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base.Event
{
    /// <summary>
    /// 标准通用事件格式
    /// </summary>
    public class KonataEventArgs:EventArgs
    {
        /// <summary>
        /// 发起者实体
        /// </summary>
        public Entity Entity { get; set; }

        /// <summary>
        /// 负载
        /// </summary>
        public object Data { get; set; }
    }
}
