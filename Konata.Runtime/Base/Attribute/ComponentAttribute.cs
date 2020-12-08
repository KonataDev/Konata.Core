namespace Konata.Runtime.Base
{
    /// <summary>
    /// 组件类型特性
    /// </summary>
    public class ComponentAttribute : BaseAttribute
    {
        public ComponentAttribute(string name = "UnDefined", string description = "")
            : base(name, description)
        {

        }
    }
}
