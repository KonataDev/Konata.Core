using System;

namespace Konata.Core.Packet
{
    /// <summary>
    /// SSO Service Attribute
    /// </summary>
    public class SSOServiceAttribute : Attribute
    {
        public string ServiceName { get; set; } = "";

        public string Description { get; set; } = "";

        public SSOServiceAttribute(string name, string description)
        {
            ServiceName = name;
            Description = description;
        }
    }
}
