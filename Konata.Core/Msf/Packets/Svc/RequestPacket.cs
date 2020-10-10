using System;
using Konata.Utils.Jce;

namespace Konata.Msf.Packets.Svc
{
    class RequestPacket : JceOutputStream
    {
        public byte _packetType = 0;
        public int _messageType = 0;
        public int _requestId = 0;
        public int _timeout = 0;
        public short _version = 0;
        public string _funcName = null;
        public string _servantName = null;

        public RequestPacket()
        {

        }

        public void WriteTo(JceOutputStream os)
        {
            os.Write(_version, 1);
            os.Write(_packetType, 2);
            os.Write(_messageType, 3);
            os.Write(_requestId, 4);
            os.Write(_servantName, 5);
            os.Write(_funcName, 6);
            //os.Write(_buffer, 7);
            //os.Write(_timeout, 8);
            //os.Write(_context, 9);
            //os.Write(_status, 10);
        }



    }

    public class RequestPacketBody : JceOutputStream
    {
        public RequestPacketBody()
            : base()
        {

        }
    }
}
