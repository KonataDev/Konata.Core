using System;
using Konata.Msf;
using Konata.Msf.Network;
using Konata.Msf.Packets;

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

        internal uint PostPacket()
        {


            return _ssoSeq;
        }

        internal uint SendSsoMessage(Packet packet)
        {
            var ssoPacket = GetSsoHeader();
            // <TODO> get packet content and create sso packet
            _pakman.Emit(ssoPacket);
            return _ssoSeq;

        }

        internal void WaitForMessage(uint ssoSeq)
        {

        }

        private Packet GetSsoHeader()
        {
            return new Packet();
        }

        private void HandleSsoMessage(byte[] fromService)
        {
            // <TODO> unpack bytes and update fields and pass remain bytes to ServiceRoutine
        }

        private SsoPacket MakeSsoMessage(Packet packet)
        {
            return new SsoPacket();
        }

    }
}
