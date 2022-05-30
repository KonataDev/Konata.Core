using System;
using Konata.Core.Common;

namespace Konata.Core.Packets.Protobuf.Highway;

internal class PicUpEcho : PicUp
{
    public const string Command = "PicUp.Echo";

    public PicUpEcho(AppInfo appInfo, uint peerUin, int sequence)
        : base(Command, 0, appInfo, peerUin, sequence)
    {
    }
}
