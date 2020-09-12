using System;
using Konata.Msf;
using Konata.Msf.Network;
using Konata.Msf.Packets;

namespace konata.Msf
{
    internal class SsoMan
    {

        private PacketMan _pakman;

        private uint _ssoSquence = 85600;

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

        internal uint PostMessage()
        {

            return _ssoSquence;
        }

        internal uint SendMessage(Service service, Packet packet)
        {
            var ssoPacket = new SsoPacket(_ssoSquence, _ssoSession, service.name, packet);
            // <TODO> get packet content and create sso packet
            _pakman.Emit(ssoPacket);
            return _ssoSquence;

        }

        internal void WaitForMessage(uint ssoSeq)
        {

        }

        private void HandleSsoMessage(byte[] fromService)
        {
            // <TODO> unpack bytes and update fields and pass remain bytes to ServiceRoutine
        }

    }
}
