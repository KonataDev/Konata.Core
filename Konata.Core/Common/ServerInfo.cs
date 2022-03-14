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
    
    public ServerInfo(string host, string port)
    {
        Host = host;
        Port = ushort.Parse(port);
    }

    public override string ToString()
        => $"{Host}:{Port}";
}
