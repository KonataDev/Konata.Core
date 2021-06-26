using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

using Konata.Utils.Crypto;
using Konata.Utils.Protobuf;
using Konata.Core.Packet.Protobuf;

namespace Konata.Core
{
    public class BotKeyStore
    {
        public User Account { get; set; }

        public WtLogin Session { get; set; }

        public class User
        {
            public uint Uin { get; set; }

            public string Name { get; set; }
                = "";

            public int Face { get; set; }

            public byte[] PasswordMd5 { get; set; }
                = new byte[] { };

            public byte[] SyncCookie { get; set; }
                = new byte[] { };
        }

        public class WtLogin
        {
            public byte[] GSecret { get; set; }
                = new byte[] { };

            public string DSecret { get; set; }
                = "";

            public byte[] TgtKey { get; set; }
                = new byte[] { };

            public byte[] TgtToken { get; set; }
                = new byte[] { };

            public byte[] D2Key { get; set; }
                = new byte[] { };

            public byte[] D2Token { get; set; }
                = new byte[] { };

            public byte[] GtKey { get; set; }
                = new byte[] { };

            public byte[] StKey { get; set; }
                = new byte[] { };

            public byte[] Tlv106Key { get; set; }
                = new byte[] { };
        }

        public BotKeyStore()
        {

        }

        public BotKeyStore(string uin, string password, string imei)
        {
            var uinNum = uint.Parse(uin);
            var dPassword = MakeDpassword();
            var passwordMd5 = new Md5Cryptor()
                .Encrypt(Encoding.UTF8.GetBytes(password));

            Account = new User
            {
                Uin = uinNum,
                PasswordMd5 = passwordMd5,
                SyncCookie = MakeSyncCookie()
            };

            Session = new WtLogin
            {
                DSecret = dPassword,
                GSecret = MakeGSecret(imei, dPassword, null),
                Tlv106Key = new Md5Cryptor().Encrypt(passwordMd5
                    .Concat(new byte[] { 0x00, 0x00, 0x00, 0x00 })
                    .Concat(BitConverter.GetBytes(uinNum).Reverse()).ToArray())
            };
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

        private static string MakeDpassword()
        {
            try
            {
                var random = new Random();
                var seedTable = new byte[16];

                bool RandBoolean()
                {
                    return random.Next(0, 1) == 1;
                }

                using (RNGCryptoServiceProvider SecurityRandom =
                    new RNGCryptoServiceProvider())
                {
                    SecurityRandom.GetBytes(seedTable);
                }

                for (int i = 0; i < seedTable.Length; ++i)
                {
                    seedTable[i] = (byte)(Math.Abs(seedTable[i] % 26) + (RandBoolean() ? 97 : 65));
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
            return ProtoTreeRoot.Serialize
                (new SyncCookie(DateTimeOffset.UtcNow.ToUnixTimeSeconds())).GetBytes();
        }
    }
}
