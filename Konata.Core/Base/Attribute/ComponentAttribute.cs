using Konata.Core.Base.ComponentType;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base
{
    /// <summary>
    /// 组件类型特性标签
    /// </summary>
    public class ComponentAttribute:BaseAttribute
    {
        public ComponentMode Mode { get; private set; }

        public ComponentAttribute(ComponentMode mode)
        {
            this.Mode = mode;
        }
    }
}
