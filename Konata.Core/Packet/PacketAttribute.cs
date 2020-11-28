using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Packet
{
    /// <summary>
    /// 报文解析服务标签
    /// </summary>
    public class PacketAttribute:Attribute
    {
        public string PacketName { get; set; } = "";

        public string Description { get; set; } = "";
        public PacketAttribute(string name,string description)
        {
            PacketName = name;
            Description = description;
        }
    }
}
