using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Base
{
    /// <summary>
    /// 服务类型特性
    /// </summary>
    public class ServiceAttribute:BaseAttribute
    {
        public ServiceAttribute(string name="Undefined",string des="")
            :base(name,des)
        {

        }
    }
}
