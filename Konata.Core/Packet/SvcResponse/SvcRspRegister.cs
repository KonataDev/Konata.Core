using System;
using Konata.Core.Packet.Wup;
using Konata.Utils.JceStruct.Model;
namespace Konata.Core.Packet.SvcResponse
{
    public class SvcRspRegister : UniPacket
    {
        public long uin;
        public long bid;
        public string ipAddress;
        public bool status;

        public SvcRspRegister(byte[] response)
            : base(response, (object userdata, JStruct r) =>
            {
                var p = (SvcRspRegister)userdata;
                {
                    p.uin = (JNumber)r["0.0"];
                    p.bid = (JNumber)r["0.1"];
                    p.status = (JNumber)r["0.9"] == 1;
                    p.ipAddress = (string)(JString)r["0.10"];
                }
            })
        {

        }
    }
}
