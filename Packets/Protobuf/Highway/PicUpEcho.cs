using System;

namespace Konata.Core.Packets.Protobuf.Highway;

internal class PicUpEcho : PicUp
{
    public const string Command = "PicUp.Echo";

    public PicUpEcho(uint peerUin, int sequence)
        : base(Command, 0, peerUin, sequence)
    {
    }
}
