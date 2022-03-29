using System;

namespace Konata.Core.Utils.Network.TcpClient;

internal interface IClientListener
{
    /// <summary>
    /// Dissect a stream
    /// </summary>
    /// <returns></returns>
    public uint OnStreamDissect(byte[] data, uint length);

    /// <summary>
    /// On handle a packet
    /// </summary>
    /// <param name="data"></param>
    public void OnRecvPacket(byte[] data);

    /// <summary>
    /// On client disconnect
    /// </summary>
    public void OnDisconnect();

    /// <summary>
    /// On socket error
    /// </summary>
    public void OnSocketError(Exception e);
}
