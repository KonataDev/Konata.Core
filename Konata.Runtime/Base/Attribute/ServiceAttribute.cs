using System;

namespace Konata.Runtime.Base
{
    /// <summary>
    /// 服务类型特性
    /// </summary>
    public class ServiceAttribute : BaseAttribute
    {
        public ServiceAttribute(string name = "Undefined", string des = "")
            : base(name, des)
        {

        }
    }
}
