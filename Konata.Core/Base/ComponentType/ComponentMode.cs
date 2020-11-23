using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base.ComponentType
{
    /// <summary>
    /// 组件的加载模式
    /// </summary>
    [Flags]
    public enum ComponentMode
    {
        /// <summary>
        /// 无模式,纯使用者单独控制
        /// </summary>
        None=0,
        /// <summary>
        /// 包含全部模式
        /// [需要实现所有ComponentType下的接口类型]
        /// </summary>
        All = 1<<0,
        /// <summary>
        /// 存在组件加载时需要处理的模式
        /// [需要实现ILoad]
        /// </summary>
        Load=1<<1,
        /// <summary>
        /// 存在组件启动时需要处理的模式
        /// [需要实现IStart]
        /// </summary>
        Start = 1<<2,
    }
}
