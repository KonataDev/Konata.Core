using System;

namespace Konata.Core
{
    /// <summary>
    /// Konata Component Attribute
    /// </summary>
    internal class ComponentAttribute : Attribute
    {
        public string ComponentName { get; set; }

        public string Description { get; set; }

        public ComponentAttribute(string name, string description)
        {
            ComponentName = name;
            Description = description;
        }
    }
}
