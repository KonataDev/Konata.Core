using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Attributes
{
    /// <summary>
    /// SSO Service Attribute
    /// </summary>
    internal class ServiceAttribute : Attribute
    {
        public string ServiceName { get; }

        public string Description { get; }

        public ServiceAttribute(string name, string description)
        {
            ServiceName = name;
            Description = description;
        }
    }
}
