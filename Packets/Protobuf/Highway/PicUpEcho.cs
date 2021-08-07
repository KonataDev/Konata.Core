using System;

namespace Konata.Core.Packets.Protobuf.Highway
{
    public class PicUpEcho : PicUp
    {
        public PicUpEcho(uint peerUin, int sequence)
            : base("PicUp.Echo", 0, peerUin, sequence)
        {

        }
    }
}
