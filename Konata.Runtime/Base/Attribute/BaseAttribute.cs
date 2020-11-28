using System;

namespace Konata.Runtime.Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BaseAttribute : System.Attribute
    {
        public Type AttributeType { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public BaseAttribute(string name = "UnDefined", string des = "")
        {
            this.AttributeType = this.GetType();
            this.Name = name;
            this.Description = des;
        }
    }
}
