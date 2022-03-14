namespace Konata.Core.Utils.Network;

/// <summary>
/// Networking tools
/// </summary>
internal static class NetTool
{
    public static string UintToIPBE(uint ip)
        => $"{(ip >> 0) & 0xFF}." + $"{(ip >> 8) & 0xFF}." +
           $"{(ip >> 16) & 0xFF}." + $"{(ip >> 24) & 0xFF}";
}
