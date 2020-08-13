using System;
using Konata.Protocol.Packet;
using Konata.Protocol.Packet.Oicq;
using Konata.Protocol.Utils;

using SsoCommand = Konata.Protocol.SsoServiceCmd.Command;

namespace Konata
{

    public class Bot
    {
        private long _uin;
        private string _passsword;
        private bool _isLogin;

        private byte[] _tgtgKey =
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private byte[] _randKey =
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private byte[] _shareKey =
            { 0x4D, 0xA0, 0xF6, 0x14, 0xFC, 0x9F, 0x29, 0xC2, 0x05, 0x4C, 0x77, 0x04, 0x8A, 0x65, 0x66, 0xD7 };
        private byte[] _zeroKey =
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public Bot(long botUin, string botPassword)
        {
            _uin = botUin;
            _passsword = botPassword;
        }

        public Bot(string botUin, string botPassword)
        {
            _uin = Convert.ToInt64(botUin);
            _passsword = botPassword;
        }

        public void Login()
        {
            SsoPacket ssoPacket =
                SsoPacketFactory.Build(SsoCommand.WtLoginAuth, new OicqRequestTgtgt(_uin, _passsword, _tgtgKey, _randKey, _shareKey));

            ToServicePacket toServicePacket =
                ToServicePacketFactory.Build(ssoPacket, 0x0A, 0x02, _zeroKey, _uin);

            Console.WriteLine(toServicePacket.ToString());
        }
    }
}
