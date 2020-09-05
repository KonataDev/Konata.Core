using System;
using Konata.Utils;

namespace Konata.Msf.Packets.Oicq
{
    public abstract class OicqRequest : Packet
    {
        protected short _cmd;
        protected short _subCmd;
    }
}
