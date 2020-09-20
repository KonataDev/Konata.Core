using System;

namespace Konata.Msf.Packets.Oicq
{
    public class OicqRequestCheckSms : OicqRequest
    {
        public OicqRequestCheckSms(uint uin)
            : base(0x0810, 0x07, uin)
        {

        }
    }
}
