using System.Net.NetworkInformation;

namespace Konata.Core.Utils.Network;

internal static class Icmp
{
    /// <summary>
    /// Pinging IP
    /// </summary>
    /// <param name="hostIp"><b>[In] </b>Host IP address</param>
    /// <param name="timeout"><b>[Opt] </b>Pinging timeout default 1000 ms</param>
    /// <returns></returns>
    public static long Ping(string hostIp, int timeout = 1000)
    {
        using var ping = new Ping();
        var reply = ping.Send(hostIp, timeout);
        {
            if (reply?.Status == IPStatus.Success)
                return reply.RoundtripTime;
        }
        return long.MaxValue;
    }
}
