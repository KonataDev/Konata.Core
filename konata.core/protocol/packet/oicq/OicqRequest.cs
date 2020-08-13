using System;
using System.Linq;
using System.Text;
using Konata.Utils;

namespace Konata.Protocol.Packet.Oicq
{
    public abstract class OicqRequest : PacketBase
    {
        protected short _cmd;
        protected short _subCmd;

        public override byte[] GetBytes() => null;

    }
}
