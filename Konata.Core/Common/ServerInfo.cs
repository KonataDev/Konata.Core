namespace Konata.Core.Common;

public class ServerInfo
{
    /// <summary>
    /// Server host
    /// </summary>
    public string Host { get; }

    /// <summary>
    /// Server port
    /// </summary>
    public ushort Port { get; }

    public ServerInfo(string host, ushort port)
    {
        Host = host;
        Port = port;
    }

    public override string ToString()
        => $"{Host}:{Port}";
}
