using System;

namespace Konata.Runtime.Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BaseAttribute : Attribute
    {
        public Type AttributeType { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public BaseAttribute(string name = "UnDefined", string description = "")
        {
            Name = name;
            Description = description;
            AttributeType = GetType();
        }
    }
}
