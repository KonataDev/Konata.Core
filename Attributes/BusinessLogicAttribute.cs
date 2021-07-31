using System;

namespace Konata.Core.Attributes
{
    /// <summary>
    /// Business logic Attribute
    /// </summary>
    internal class BusinessLogicAttribute : Attribute
    {
        public string LogicName { get; set; }

        public string Description { get; set; }

        public BusinessLogicAttribute(string name, string description)
        {
            LogicName = name;
            Description = description;
        }
    }
}
