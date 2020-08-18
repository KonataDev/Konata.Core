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
            { 0x2E, 0x39, 0x9A, 0x9C, 0xF2, 0x57, 0x12, 0xF8, 0x1E, 0x5B, 0x63, 0x2E, 0xB3, 0xB3, 0xF7, 0x9F };
        private readonly byte[] _randKey =
            { 0xE2, 0xED, 0x53, 0x77, 0xAD, 0xFD, 0x99, 0x83, 0x56, 0xEB, 0x8B, 0x4C, 0x62, 0x7C, 0x22, 0xC4 };
        private readonly byte[] _shareKey =
            { 0x4D, 0xA0, 0xF6, 0x14, 0xFC, 0x9F, 0x29, 0xC2, 0x05, 0x4C, 0x77, 0x04, 0x8A, 0x65, 0x66, 0xD7 };
        private readonly byte[] _zeroKey =
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private readonly byte[] _defaultPublicKey =
            { 0x02, 0x0B, 0x03, 0xCF, 0x3D, 0x99, 0x54, 0x1F,
              0x29, 0xFF, 0xEC, 0x28, 0x1B, 0xEB, 0xBD, 0x4E,
              0xA2, 0x11, 0x29, 0x2A, 0xC1, 0xF5, 0x3D, 0x71, 0x28
            };

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
                SsoPacketFactory.Build(SsoCommand.WtLoginAuth, new OicqRequestTgtgt(_uin, _password, _tgtgKey, _randKey, _shareKey, _defaultPublicKey));

            ToServicePacket toServicePacket =
                ToServicePacketFactory.Build(ssoPacket, 0x0A, 0x02, _zeroKey, _uin);

            _pakman.Emit(toServicePacket);

            // Console.WriteLine(toServicePacket.ToString());
        }


        private void PacketListener(FromServicePacket packet)
        {

        }

    }
}
