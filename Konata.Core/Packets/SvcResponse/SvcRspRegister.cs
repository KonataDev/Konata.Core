using System;
using Konata.Packets.Wup;
using Konata.Library.JceStruct;

namespace Konata.Packets.SvcResponse
{
    public class SvcRspRegister : UniPacket
    {
        public long uin;
        public long bid;
        public string ipAddress;
        public bool status;

        public SvcRspRegister(byte[] response)
            : base(response, (object userdata, Jce.Struct r) =>
            {
                var p = (SvcRspRegister)userdata;
                {
                    p.uin = (Jce.Number)r["0.0"];
                    p.bid = (Jce.Number)r["0.1"];
                    p.status = (Jce.Number)r["0.9"] == 1;
                    p.ipAddress = (string)(Jce.String)r["0.10"];
                }
            })
        {

        }
    }
}
