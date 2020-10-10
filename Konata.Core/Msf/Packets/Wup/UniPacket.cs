using System;
using Konata.Utils.Jce;
using Konata.Msf.Packets.Svc;

namespace Konata.Msf.Packets.Wup.UniPacket
{
    public class UniPacket
    {
        private RequestPacket _package;

        public UniPacket()
        {
            _package = new RequestPacket();
            _package._version = 2;
        }

        public UniPacket(bool useVersion3)
        {
            _package = new RequestPacket();
            _package._version = (short)(useVersion3 ? 3 : 2);
        }

        public void SetServantName(string name)
        {
            _package._servantName = name;
        }

        public void SetFuncName(string name)
        {
            _package._funcName = name;
        }

        public void Put(string name, JceOutputStream os)
        {
        }

    }
}
