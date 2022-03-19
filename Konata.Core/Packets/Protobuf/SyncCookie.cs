using System;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UseDeconstructionOnParameter

namespace Konata.Core.Packets.Protobuf;

internal class SyncCookie : ProtoTreeRoot
{
    public SyncCookie((uint, uint) consts)
    {
        var epoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random();

        AddLeafVar("08", epoch);
        AddLeafVar("10", epoch);
        AddLeafVar("18", consts.Item1);
        AddLeafVar("20", consts.Item2);
        AddLeafVar("28", random.Next());
        AddLeafVar("48", random.Next());
        AddLeafVar("58", random.Next());
        AddLeafVar("60", consts.Item1 & 0xFF);
        AddLeafVar("68", epoch);
        AddLeafVar("70", 0);
    }
}
