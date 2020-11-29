using System;

namespace Konata.Runtime.Base
{
    /// <summary>
    /// 服务类型特性
    /// </summary>
    public class ServiceAttribute : BaseAttribute
    {
        public ServiceAttribute(string name = "Undefined", string description = "")
            : base(name, description)
        {

        }
    }
}
