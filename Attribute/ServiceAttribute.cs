using System;

namespace Konata.Core
{
    /// <summary>
    /// SSO Service Attribute
    /// </summary>
    internal class ServiceAttribute : Attribute
    {
        public string ServiceName { get; set; }

        public string Description { get; set; }

        public ServiceAttribute(string name, string description)
        {
            ServiceName = name;
            Description = description;
        }
    }
}
