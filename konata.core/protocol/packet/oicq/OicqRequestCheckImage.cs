using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Protocol.Packet.Oicq
{
    public class OicqRequestCheckImage : OicqRequest
    {
        public OicqRequestCheckImage()
        {
            cmd = 0x0810;
            subCmd = 0x02;
            serviceCmd = "wtlogin.login";
        }

    }
}
