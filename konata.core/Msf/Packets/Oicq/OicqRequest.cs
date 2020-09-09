using System;
using Konata.Utils;

namespace Konata.Msf.Packets.Oicq
{
    public abstract class OicqRequest : Packet
    {
        protected ushort _cmd;
        protected ushort _subCmd;
    }
}
