using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konata.Protocol.Packet;
using Konata.Protocol.Packet.Oicq;
using Konata.Protocol.Packet.Tlvs;
using Konata.Protocol.Utils;
using Konata.Utils;

namespace Konata
{

    public class Bot
    {
        private ulong uin;
        private string passsword;
        private bool isLogin;

        private byte[] tgtgKey =
            {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

        public Bot(ulong botUin, string botPassword)
        {
            uin = botUin;
            passsword = botPassword;
        }

        public Bot(string botUin, string botPassword)
        {
            uin = Convert.ToUInt64(botUin);
            passsword = botPassword;
        }

        public void Login()
        {
            PacketBase ssoPacket = new SsoBuilder(new OicqRequestTgtgt(uin, passsword, tgtgKey)).GetPacket();
            Console.WriteLine(ssoPacket.ToString());
        }
    }


}
