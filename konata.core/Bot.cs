using System;
using Konata.Protocol.Packet;
using Konata.Protocol.Packet.Oicq;
using Konata.Protocol.Utils;

using SsoCommand = Konata.Protocol.SsoServiceCmd.Command;

namespace Konata
{

    public class Bot
    {
        private long uin;
        private string passsword;
        private bool isLogin;

        private byte[] tgtgKey =
            {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
        private byte[] randKey =
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private byte[] shareKey =
            { 0x4D, 0xA0, 0xF6, 0x14, 0xFC, 0x9F, 0x29, 0xC2, 0x05, 0x4C, 0x77, 0x04, 0x8A, 0x65, 0x66, 0xD7 };

        public Bot(long botUin, string botPassword)
        {
            uin = botUin;
            passsword = botPassword;
        }

        public Bot(string botUin, string botPassword)
        {
            uin = Convert.ToInt64(botUin);
            passsword = botPassword;
        }

        public void Login()
        {
            SsoPacket ssoPacket =
                SsoPacketFactory.Build(SsoCommand.WtLoginAuth, new OicqRequestTgtgt(uin, passsword, tgtgKey, shareKey, randKey));



            Console.WriteLine(ssoPacket.ToString());
        }
    }


}
