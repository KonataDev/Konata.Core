using System;

namespace Konata.Msf.Packets.Oicq
{
    public class OicqRequestCheckImage : OicqRequest
    {
        public OicqRequestCheckImage(uint uin)
            : base(0x0810, 0x02, uin)
        {

        }
    }
}
