using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Protocol.Packet.Oicq
{
    public class OicqRequestCheckSms : OicqRequest
    {
        public OicqRequestCheckSms()
        {
            cmd = 0x0810;
            subCmd = 0x07;
            serviceCmd = "wtlogin.login";
        }
    }
}
