using System;
using System.Linq;
using System.Text;
using Konata.Utils;

namespace Konata.Protocol.Packet.Oicq
{
    public abstract class OicqRequest : PacketBase
    {
        protected short cmd;
        protected short subCmd;
        protected string serviceCmd;

        public override byte[] GetBytes() => null;

    }
}
