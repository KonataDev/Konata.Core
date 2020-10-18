using System;
using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Protobuf
{
    class SyncCookie : ProtoTreeRoot
    {
        public SyncCookie(long timeStamp)
        {
            addLeafVar("08", timeStamp);
            addLeafVar("10", timeStamp);
            addLeafVar("28", 2267374858);
            addLeafVar("48", 1657171111);
            addLeafVar("58", 1828320251);
            addLeafVar("68", timeStamp);
            addLeafVar("70", 0);
        }
    }
}
