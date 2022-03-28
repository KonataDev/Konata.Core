using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Packets.SvcResponse;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.Extensions;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Network;
using Konata.Core.Utils.TcpSocket;

// ReSharper disable InvertIf
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable ParameterTypeCanBeEnumerable.Global
// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

namespace Konata.Core.Components;

[Component("SocketComponent", "Konata Socket Client Component")]
internal class SocketComponent : InternalComponent, IClientListener
{
    public bool Connected
        => _tcpClient.Connected;

    private const string TAG = "SocketComponent";
    private readonly AsyncClient _tcpClient;

    public SocketComponent()
    {
        _tcpClient = new(this);
    }

    /// <summary>
    /// Connect to server
    /// </summary>
    /// <param name="useLowLatency"><b>[Opt] </b>Auto select the fastest server to connect.</param>
    /// <returns></returns>
    public async Task<bool> Connect(bool useLowLatency = false)
    {
        // Check socket
        if (_tcpClient.Connected)
        {
            LogW(TAG, "Calling Connect method after socket connected.");
            return true;
        }

        var lowestTime = long.MaxValue;
        var serverList = new ((string, int), long)[] { }.ToList();

        // Using user config
        if (ConfigComponent.GlobalConfig!.CustomHost != null)
        {
            // Parse the config
            var customHost = ConfigComponent
                .GlobalConfig.CustomHost.Split(':');

            // Connect to server with
            // custom server address
            if (customHost.Length >= 1)
            {
                return await _tcpClient.Connect(customHost[0],
                    customHost.Length == 2 ? ushort.Parse(customHost[1]) : 8080);
            }

            // Failed to parse the config
            LogE(TAG, "Invalid custom host config passed in.");
            return false;
        }

        // Using fastest server
        else if (useLowLatency)
        {
            var servers = await GetServerList();

            // Ping the server
            foreach (var server in servers)
            {
                var time = Icmp.Ping(server.Host, 2000);
                {
                    if (time < lowestTime) lowestTime = time;
                    serverList.Add(((server.Host, server.Port), time));
                }

                LogI(TAG, "Testing latency " +
                          $"{server.Host}:{server.Port} " +
                          $"=> {time}ms.");
            }

            // Sort the list by lantency
            serverList.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            var tryQueue = new Queue(serverList);

            // Try connect to each server
            while (tryQueue.Count > 0)
            {
                var pop = (((string, int ) Host, long Latency)) tryQueue.Dequeue();
                var (addr, port) = pop.Host;

                // Connect
                LogI(TAG, $"Try Connecting {addr}:{port}.");
                var result = await _tcpClient.Connect(addr, port);

                // Try next server
                if (result) return true;
                {
                    LogI(TAG, $"Failed to connecting to {addr}:{port}.");
                    await _tcpClient.Disconnect();
                }
            }
        }

        throw new Exception("All servers are unavailable.");
    }

    /// <summary>
    /// Reconnect
    /// </summary>
    /// <returns></returns>
    public Task<bool> Reconnect()
        => _tcpClient.Reconnect();

    /// <summary>
    /// Disconnect from server
    /// </summary>
    /// <param name="reason"></param>
    public async Task<bool> Disconnect(string reason)
    {
        // Not connected
        if (_tcpClient is not {Connected: true})
        {
            LogW(TAG, "Calling Disconnect " +
                      "method after socket disconnected.");
            return true;
        }

        // Disconnect
        LogI(TAG, $"Disconnect, Reason => {reason}");
        return await _tcpClient.Disconnect();
    }

    /// <summary>
    /// On dissecting a packet
    /// </summary>
    /// <param name="data"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public uint OnStreamDissect(byte[] data, uint length)
    {
        // Lack more data
        if (length < 4) return 0;

        // Teardown the packet length
        return BitConverter.ToUInt32
            (data.Take(4).Reverse().ToArray(), 0);
    }

    /// <summary>
    /// On Received a packet 
    /// </summary>
    /// <param name="data"></param>
    public void OnRecvPacket(byte[] data)
    {
        var packet = new byte[data.Length];
        Array.Copy(data, 0, packet, 0, data.Length);
        {
            // LogV(TAG, $"Recv data => \n  {ByteConverter.Hex(packet, true)}");
            PostEvent<PacketComponent>(PacketEvent.Push(data));
        }
    }

    /// <summary>
    /// On disconnect
    /// </summary>
    public void OnDisconnect()
        => LogI(TAG, "Client disconnected.");

    /// <summary>
    /// On socket error
    /// </summary>
    /// <param name="e"></param>
    public void OnSocketError(Exception e)
        => LogE(TAG, e);

    /// <summary>
    /// Event handler
    /// </summary>
    /// <param name="anyEvent"></param>
    /// <returns></returns>
    public override async Task<bool> OnHandleEvent(BaseEvent anyEvent)
    {
        if (anyEvent is not PacketEvent packetEvent)
        {
            LogW(TAG, "Unsupported event received.");
            return false;
        }

        // Not connected
        if (_tcpClient is not {Connected: true})
            LogW(TAG, "Calling SendData method after socket disconnected.");

        // Send data
        return await _tcpClient.Send(packetEvent.Buffer);
        // LogV(TAG, $"Send data => \n  {ByteConverter.Hex(packetEvent.Buffer, true)}");
    }

    /// <summary>
    /// Get server list
    /// </summary>
    /// <returns></returns>
    private async Task<IEnumerable<ServerInfo>> GetServerList()
    {
        // Make request packet
        var encKey = "F0441F5FF42DA58FDCF7949ABA62D411".UnHex();
        var reqUrl = "https://configsvr.msf.3g.qq.com/configsvr/serverlist.jsp";
        var reqBuf = new PacketBase();
        {
            reqBuf.EnterBarrierEncrypted(ByteBuffer.Prefix.None,
                Endian.FollowMachine, TeaCryptor.Instance, encKey);
            {
                var body = new SvcReqHttpServerListReq();
                reqBuf.PutUintBE(body.Length + 4);
                reqBuf.PutByteBuffer(body);
            }
            reqBuf.LeaveBarrier();
        }

        try
        {
            // Request server list
            var response = await Http.Post(reqUrl, reqBuf.GetBytes());
            {
                var rspBuf = new PacketBase(response, TeaCryptor.Instance, encKey);
                {
                    rspBuf.EatBytes(4);
                    return new SvcRspHttpServerListRsp(rspBuf.TakeAllBytes(out _)).Servers;
                }
            }
        }
        catch (Exception e)
        {
            LogE(TAG, e);
            LogW(TAG, "Request server list failed, fallback to the default server.");
            return new ServerInfo[] {new("msfwifi.3g.qq.com", 8080)};
        }
    }
}
