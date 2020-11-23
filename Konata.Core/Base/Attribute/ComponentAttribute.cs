using Konata.Core.Base.ComponentType;
using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base
{
    public class ComponentAttribute:BaseAttribute
    {
        public ComponentMode Mode { get; private set; }

        public ComponentAttribute(ComponentMode mode)
        {
            this.Mode = mode;
        }
    }
}
