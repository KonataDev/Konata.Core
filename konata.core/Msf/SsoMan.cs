using System;
using Konata.Msf.Network;

namespace konata.Msf
{
    internal class SsoMan
    {

        private PacketMan _pakman;

        private uint _ssoSeq = 85600;

        private uint _ssoSession = 0x01DAA2BC;

        internal SsoMan()
        {
            _pakman = new PacketMan();
        }

        internal bool Initialize()
        {
            _pakman.OpenSocket();
            return true;
        }

        internal void PostPacket()
        {

        }

        internal void SendPacket()
        {

        }

    }
}
