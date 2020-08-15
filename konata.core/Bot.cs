using System;
using System.Net.Sockets;
using Konata.Network;
using Konata.Protocol.Packet;
using Konata.Protocol.Packet.Oicq;
using Konata.Protocol.Utils;

using SsoCommand = Konata.Protocol.SsoServiceCmd.Command;

namespace Konata
{

    public class Bot
    {
        private readonly long _uin;
        private readonly string _password;
        private bool _isLogin;
        private PacketMan _pakman;

        private readonly byte[] _tgtgKey =
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private readonly byte[] _randKey =
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private readonly byte[] _shareKey =
            { 0x4D, 0xA0, 0xF6, 0x14, 0xFC, 0x9F, 0x29, 0xC2, 0x05, 0x4C, 0x77, 0x04, 0x8A, 0x65, 0x66, 0xD7 };
        private readonly byte[] _zeroKey =
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public Bot(long botUin, string botPassword)
        {
            _uin = botUin;
            _password = botPassword;

            _pakman = new PacketMan(PacketListener);
            _pakman.Init();
        }

        public Bot(string botUin, string botPassword)
        {
            _uin = Convert.ToInt64(botUin);
            _password = botPassword;

            _pakman = new PacketMan(PacketListener);
            _pakman.Init();
        }

        public void Login()
        {

            SsoPacket ssoPacket =
                SsoPacketFactory.Build(SsoCommand.WtLoginAuth, new OicqRequestTgtgt(_uin, _password, _tgtgKey, _randKey, _shareKey));

            ToServicePacket toServicePacket =
                ToServicePacketFactory.Build(ssoPacket, 0x0A, 0x02, _zeroKey, _uin);

            _pakman.Emit(toServicePacket);

            Console.WriteLine(toServicePacket.ToString());
        }


        private void PacketListener(PacketBase packet)
        {

        }

    }
}
