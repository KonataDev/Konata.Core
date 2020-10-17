using System;
using Konata.Library.Protobuf;

namespace Konata.Msf.Packets.Protobuf
{
    public class ProtoGetMsg : ProtoTreeRoot
    {
        public ProtoGetMsg()
        {
            addLeafVar("08", 0);
            addTree("12", (ProtoTreeRoot cookies) =>
            {
                cookies.addLeafVar("08", 1602783217);
                cookies.addLeafVar("10", 1602783217);
                cookies.addLeafVar("28", 2267374858);
                cookies.addLeafVar("48", 1657171111);
                cookies.addLeafVar("58", 1828320251);
                cookies.addLeafVar("68", 1602783217);
                cookies.addLeafVar("70", 0);
            });
            addLeafVar("18", 0);
            addLeafVar("20", 20);
            addLeafVar("28", 3);
            addLeafVar("30", 1);
            addLeafVar("38", 1);
            addLeafVar("48", 0);
            addLeafBytes("62", null);
        }
    }
}
