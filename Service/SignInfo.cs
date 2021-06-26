using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Konata.Utils.Crypto;
using Konata.Core.Packet.Oicq;
using Konata.Core.Packet.Protobuf;
using Konata.Utils.Protobuf;

namespace Konata.Core.Service
{
    public class SignInfo
    {
        internal class UserInfo : BotKeyStore.User
        {
            public int Age { get; set; }
        }

        internal class WtLoginInfo : BotKeyStore.WtLogin
        {
            public byte[] WtSessionTicketSig { get; set; }
                = new byte[] { };

            public byte[] WtSessionTicketKey { get; set; }
                = new byte[] { };

            public string WtLoginSmsToken { get; set; }

            public string WtLoginSmsPhone { get; set; }

            public string WtLoginSmsCountry { get; set; }

            public string WtLoginSession { get; set; }
        }

        internal UserInfo Account { get; set; }

        internal WtLoginInfo Session { get; set; }

        #region Keys And Tokens

        public byte[] TgtgKey { get; private set; } =
        {
            0x2E, 0x39, 0x9A, 0x9C, 0xF2, 0x57, 0x12, 0xF8,
            0x1E, 0x5B, 0x63, 0x2E, 0xB3, 0xB3, 0xF7, 0x9F
        };

        public byte[] RandKey { get; private set; } =
        {
            0xE2, 0xED, 0x53, 0x77, 0xAD, 0xFD, 0x99, 0x83,
            0x56, 0xEB, 0x8B, 0x4C, 0x62, 0x7C, 0x22, 0xC4
        };

        internal byte[] ShareKey { get; } =
        {
            0x4D, 0xA0, 0xF6, 0x14, 0xFC, 0x9F, 0x29, 0xC2,
            0x05, 0x4C, 0x77, 0x04, 0x8A, 0x65, 0x66, 0xD7
        };

        internal byte[] ZeroKey { get; } =
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        internal byte[] DefaultPublicKey { get; } =
        {
            0x02, 0x0B, 0x03, 0xCF, 0x3D, 0x99, 0x54, 0x1F,
            0x29, 0xFF, 0xEC, 0x28, 0x1B, 0xEB, 0xBD, 0x4E,
            0xA2, 0x11, 0x29, 0x2A, 0xC1, 0xF5, 0x3D, 0x71,
            0x28
        };

        internal byte[] ServerPublicKey { get; } =
        {
            0x04, 0x92, 0x8D, 0x88, 0x50, 0x67, 0x30, 0x88,
            0xB3, 0x43, 0x26, 0x4E, 0x0C, 0x6B, 0xAC, 0xB8,
            0x49, 0x6D, 0x69, 0x77, 0x99, 0xF3, 0x72, 0x11,
            0xDE, 0xB2, 0x5B, 0xB7, 0x39, 0x06, 0xCB, 0x08,
            0x9F, 0xEA, 0x96, 0x39, 0xB4, 0xE0, 0x26, 0x04,
            0x98, 0xB5, 0x1A, 0x99, 0x2D, 0x50, 0x81, 0x3D,
            0xA8
        };
        #endregion

        public SignInfo(BotKeyStore keystore)
        {
            Account = new UserInfo
            {
                Age = 0,
                Uin = keystore.Account.Uin,
                Name = keystore.Account.Name,
                Face = keystore.Account.Face,
                SyncCookie = keystore.Account.SyncCookie,
                PasswordMd5 = keystore.Account.PasswordMd5
            };

            Session = new WtLoginInfo
            {
                D2Key = keystore.Session.D2Key,
                D2Token = keystore.Session.D2Token,
                DSecret = keystore.Session.DSecret,
                GSecret = keystore.Session.GSecret,
                GtKey = keystore.Session.GtKey,
                StKey = keystore.Session.StKey,
                TgtKey = keystore.Session.TgtKey,
                TgtToken = keystore.Session.TgtToken,
                Tlv106Key = keystore.Session.Tlv106Key
            };
        }
    }
}
