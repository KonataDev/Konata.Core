using System;
using Konata.Core.Packets;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Attributes;

/// <summary>
/// SSO Service Attribute
/// </summary>
internal class ServiceAttribute : Attribute
{
    /// <summary>
    /// Service command name
    /// </summary>
    public string Command { get; }

    /// <summary>
    /// Service packet type
    /// </summary>
    public PacketType PacketType { get; }

    /// <summary>
    /// Service authorization type 
    /// </summary>
    public AuthFlag AuthType { get; }

    /// <summary>
    /// Sequence mode
    /// </summary>
    public SequenceMode SeqMode { get; }

    public ServiceAttribute(string name, PacketType pktType,
        AuthFlag authType, SequenceMode seqMode)
    {
        Command = name;
        PacketType = pktType;
        AuthType = authType;
        SeqMode = seqMode;
    }
}

/// <summary>
/// Sequence mode
/// </summary>
internal enum SequenceMode
{
    Managed = 0,
    Selfhold
}
