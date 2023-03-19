using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Threading.Tasks;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Network.TcpClient;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Packets.SvcResponse;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.Extensions;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Network;
using ClientListener = Konata.Core.Network.TcpClient.ClientListener;

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

    public uint HeaderSize => 4;

    private const string TAG = "SocketComponent";
    private readonly ClientListener _tcpClient;

    private (string, ushort) _recentlyHost;

    public SocketComponent()
    {
        _tcpClient = new CallbackClientListener(this);
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

        // Using user config
        if (Bot.GlobalConfig!.CustomHost != null)
        {
            // Parse the config
            var customHost = Bot.GlobalConfig.CustomHost.Split(':');

            // Connect to server with
            // custom server address
            if (customHost.Length >= 1)
            {
                ushort port = 8080;
                if (customHost.Length == 1 || ushort.TryParse(customHost[1], out port))
                {
                    var host = customHost[0];
                    _recentlyHost = (host, port);
                    return await _tcpClient.Connect(host, port);
                }
            }

            // Failed to parse the config
            LogE(TAG, "Invalid custom host config passed in.");
            return false;
        }

        // Using fastest server
        else if (useLowLatency)
        {
            var servers = await GetServerList();
            var serverList = new List<(string, int, long)>(servers.Count);

            // Ping the server
            foreach (var server in servers)
            {
                var time = Icmp.Ping(server.Host, 2000);
                {
                    serverList.Add((server.Host, server.Port, time));
                }

                LogI(TAG, "Testing latency " +
                          $"{server.Host}:{server.Port} " +
                          $"=> {time}ms.");
            }

            // Sort the list by latency
            serverList.Sort((a, b) => a.Item3.CompareTo(b.Item3));

            // Try connect to each server
            foreach (var (addr, port, latency) in serverList)
            {
                // Connect
                LogI(TAG, $"Try Connecting {addr}:{port}.");
                var result = await _tcpClient.Connect(addr, port);

                // Try next server
                if (result) return true;
                LogI(TAG, $"Failed to connecting to {addr}:{port}.");
            }
        }

        throw new Exception("All servers are unavailable.");
    }

    /// <summary>
    /// Reconnect
    /// </summary>
    /// <returns></returns>
    public Task<bool> Reconnect()
        => _tcpClient.Connect(_recentlyHost.Item1, _recentlyHost.Item2);

    /// <summary>
    /// Disconnect from server
    /// </summary>
    /// <param name="reason"></param>
    public Task<bool> Disconnect(string reason)
    {
        // Not connected
        if (_tcpClient is not {Connected: true})
        {
            LogW(TAG, "Calling Disconnect " +
                      "method after socket disconnected.");
        }
        else
        {
            // Disconnect
            LogI(TAG, $"Disconnect, Reason => {reason}");
            _tcpClient.Disconnect();
        }

        return Task.FromResult(true);
    }

    public uint GetPacketLength(ReadOnlySpan<byte> header)
    {
        return BinaryPrimitives.ReadUInt32BigEndian(header);
    }

    /// <summary>
    /// On Received a packet 
    /// </summary>
    public void OnRecvPacket(ReadOnlySpan<byte> packet)
    {
        var packetBuffer = packet.ToArray();
        // LogV(TAG, $"Recv data => \n  {ByteConverter.Hex(packet, true)}");
        PostEvent<PacketComponent>(PacketEvent.Push(packetBuffer));
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
    private async Task<List<ServerInfo>> GetServerList()
    {
        // Make request packet
        var encKey = "F0441F5FF42DA58FDCF7949ABA62D411".UnHex();
        var reqUrl = "https://configsvr.msf.3g.qq.com/configsvr/serverlist.jsp";
        var reqBuf = new PacketBase();
        {
            reqBuf.EnterBarrierEncrypted(ByteBuffer.Prefix.None,
                Endian.FollowMachine, TeaCryptor.Instance, encKey);
            {
                var body = new SvcReqHttpServerListReq(Bot.AppInfo);
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
            return new List<ServerInfo>(1) {new("msfwifi.3g.qq.com", 8080)};
        }
    }
}
