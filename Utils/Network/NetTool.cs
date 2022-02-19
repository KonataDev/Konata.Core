using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Konata.Core.Utils.Network
{
    /// <summary>
    /// Networking tools
    /// </summary>
    public static class NetTool
    {
        public static string UintToIPBE(uint ip)
            => $"{(ip >> 0) & 0xFF}." + $"{(ip >> 8) & 0xFF}." +
               $"{(ip >> 16) & 0xFF}." + $"{(ip >> 24) & 0xFF}";
    }
}
