using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.Protobuf;
using Konata.Core.Packets.Protobuf;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core
{
    /// <summary>
    /// Keystore
    /// </summary>
    public class BotKeyStore
    {
        /// <summary>
        /// Account
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// Session keys
        /// </summary>
        public WtLogin Session { get; set; }

        /// <summary>
        /// Fixed keys
        /// </summary>
        internal KeyStub KeyStub { get; }

        /// <summary>
        /// Highway configurations
        /// </summary>
        internal Highway Highway { get; set; }

        /// <summary>
        /// Create a key store
        /// </summary>
        public BotKeyStore()
        {
            KeyStub = new KeyStub();
            Highway = new Highway();
        }

        /// <summary>
        /// Create a key store
        /// </summary>
        /// <param name="uin"><b>[In]</b> Uin</param>
        /// <param name="password"><b>[In]</b> Password</param>
        public BotKeyStore(string uin, string password)
        {
            var uinNum = uint.Parse(uin);
            var dSecret = MakeDSecret();
            var passwordMd5 = new Md5Cryptor()
                .Encrypt(Encoding.UTF8.GetBytes(password));

            Account = new Account
            {
                Age = 0,
                Uin = uinNum,
                Name = uin,
                Face = 0,
                SyncCookie = MakeSyncCookie(),
                PasswordMd5 = passwordMd5
            };

            Session = new WtLogin
            {
                DSecret = dSecret,
                Tlv106Key = new Md5Cryptor().Encrypt(passwordMd5
                    .Concat(new byte[] {0x00, 0x00, 0x00, 0x00})
                    .Concat(BitConverter.GetBytes(uinNum).Reverse()).ToArray())
            };

            KeyStub = new KeyStub();
        }

        internal void Initial(string imei)
        {
            Session.GSecret ??= MakeGSecret(imei, Session.DSecret, null);
        }

        private static byte[] MakeGSecret(string imei, string dpwd, byte[] salt)
        {
            byte[] buffer;
            var imeiByte = Encoding.UTF8.GetBytes(imei);
            var dpwdByte = Encoding.UTF8.GetBytes(dpwd);

            if (salt != null)
            {
                buffer = new byte[imeiByte.Length + dpwdByte.Length + salt.Length];

                Array.Copy(imeiByte, buffer, imei.Length);
                Array.Copy(dpwdByte, 0, buffer, imei.Length, dpwdByte.Length);
                Array.Copy(salt, 0, buffer, imei.Length + dpwdByte.Length, salt.Length);
            }
            else
            {
                buffer = new byte[imeiByte.Length + dpwdByte.Length];

                Array.Copy(imeiByte, buffer, imei.Length);
                Array.Copy(dpwdByte, 0, buffer, imei.Length, dpwdByte.Length);
            }

            return new Md5Cryptor().Encrypt(buffer);
        }

        private static string MakeDSecret()
        {
            try
            {
                var random = new Random();
                var seedTable = new byte[16];

                bool RandBoolean()
                {
                    return random.Next(0, 1) == 1;
                }

                using (var securityRandom = new RNGCryptoServiceProvider())
                {
                    securityRandom.GetBytes(seedTable);
                }

                for (int i = 0; i < seedTable.Length; ++i)
                {
                    seedTable[i] = (byte) (Math.Abs(seedTable[i] % 26) + (RandBoolean() ? 97 : 65));
                }

                return Encoding.UTF8.GetString(seedTable);
            }
            catch
            {
                return "1234567890123456";
            }
        }

        private static byte[] MakeSyncCookie()
        {
            return ProtoTreeRoot.Serialize(new SyncCookie
                (DateTimeOffset.UtcNow.ToUnixTimeSeconds())).GetBytes();
        }
    }

    /// <summary>
    /// Account
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Account uin
        /// </summary>
        public uint Uin { get; set; }

        /// <summary>
        /// Account name
        /// </summary>
        public string Name { get; set; }
            = "";

        /// <summary>
        /// Account age
        /// </summary>
        internal int Age { get; set; }

        /// <summary>
        /// Account face id
        /// </summary>
        public int Face { get; set; }

        /// <summary>
        /// Account password md5
        /// </summary>
        public byte[] PasswordMd5 { get; set; }
            = new byte[] { };

        /// <summary>
        /// Account sync cookie
        /// </summary>
        public byte[] SyncCookie { get; set; }
            = new byte[] { };
    }

    /// <summary>
    /// WtLogin
    /// </summary>
    public class WtLogin
    {
        /// <summary>
        /// GSecret
        /// </summary>
        public byte[] GSecret { get; set; }
            = new byte[] { };

        /// <summary>
        /// DSecret
        /// </summary>
        public string DSecret { get; set; }
            = "";

        /// <summary>
        /// TgtKey
        /// </summary>
        public byte[] TgtKey { get; set; }
            = new byte[] { };

        /// <summary>
        /// TgtToken
        /// </summary>
        public byte[] TgtToken { get; set; }
            = new byte[] { };

        /// <summary>
        /// D2Key
        /// </summary>
        public byte[] D2Key { get; set; }
            = new byte[] { };

        /// <summary>
        /// D2Token
        /// </summary>
        public byte[] D2Token { get; set; }
            = new byte[] { };

        /// <summary>
        /// GtKey
        /// </summary>
        public byte[] GtKey { get; set; }
            = new byte[] { };

        /// <summary>
        /// StKey
        /// </summary>
        public byte[] StKey { get; set; }
            = new byte[] { };

        /// <summary>
        /// Tlv106Key
        /// </summary>
        public byte[] Tlv106Key { get; set; }

        internal byte[] WtSessionTicketSig { get; set; }

        internal byte[] WtSessionTicketKey { get; set; }

        internal string WtLoginSmsToken { get; set; }

        internal string WtLoginSmsPhone { get; set; }

        internal string WtLoginSmsCountry { get; set; }

        internal string WtLoginSession { get; set; }
    }

    internal class Highway
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public byte[] Ticket { get; set; }
    }

    internal class KeyStub
    {
        public byte[] TgtgKey { get; } =
        {
            0x2E, 0x39, 0x9A, 0x9C, 0xF2, 0x57, 0x12, 0xF8,
            0x1E, 0x5B, 0x63, 0x2E, 0xB3, 0xB3, 0xF7, 0x9F
        };

        public byte[] RandKey { get; } =
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
    }
}
