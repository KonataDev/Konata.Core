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
    public struct UinInfo
    {
        public uint Uin { get; set; }

        public int Age { get; set; }

        public int Face { get; set; }

        public string Name { get; set; }
    }

    public class SignInfo
    {
        public UinInfo UinInfo { get; set; }

        public byte[] PasswordMd5 { get; set; }
            = new byte[] { };

        public byte[] SyncCookie { get; set; }
            = new byte[] { };

        #region WtLogin

        public byte[] GSecret { get; set; }
            = new byte[] { };

        public string DSecret { get; set; }

        internal string WtLoginSmsToken { get; set; }

        internal string WtLoginSmsPhone { get; set; }

        internal string WtLoginSmsCountry { get; set; }

        internal string WtLoginSession { get; set; }

        #endregion

        #region Keys And Tokens

        public byte[] Tlv106Key { get; private set; }
            = new byte[] { };

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

        public byte[] ShareKey { get; private set; } =
        {
            0x4D, 0xA0, 0xF6, 0x14, 0xFC, 0x9F, 0x29, 0xC2,
            0x05, 0x4C, 0x77, 0x04, 0x8A, 0x65, 0x66, 0xD7
        };

        public byte[] ZeroKey { get; private set; } =
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        public byte[] DefaultPublicKey { get; private set; } =
        {
            0x02, 0x0B, 0x03, 0xCF, 0x3D, 0x99, 0x54, 0x1F,
            0x29, 0xFF, 0xEC, 0x28, 0x1B, 0xEB, 0xBD, 0x4E,
            0xA2, 0x11, 0x29, 0x2A, 0xC1, 0xF5, 0x3D, 0x71,
            0x28
        };

        public byte[] ServerPublicKey { get; private set; } =
        {
            0x04, 0x92, 0x8D, 0x88, 0x50, 0x67, 0x30, 0x88,
            0xB3, 0x43, 0x26, 0x4E, 0x0C, 0x6B, 0xAC, 0xB8,
            0x49, 0x6D, 0x69, 0x77, 0x99, 0xF3, 0x72, 0x11,
            0xDE, 0xB2, 0x5B, 0xB7, 0x39, 0x06, 0xCB, 0x08,
            0x9F, 0xEA, 0x96, 0x39, 0xB4, 0xE0, 0x26, 0x04,
            0x98, 0xB5, 0x1A, 0x99, 0x2D, 0x50, 0x81, 0x3D,
            0xA8
        };

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

        public byte[] WtSessionTicketSig { get; set; }
            = new byte[] { };

        public byte[] WtSessionTicketKey { get; set; }
            = new byte[] { };

        #endregion

        public SignInfo()
        {

        }

        public SignInfo(string uin, string password, string imei)
        {
            UinInfo = new UinInfo { Uin = uint.Parse(uin) };

            PasswordMd5 = new Md5Cryptor().Encrypt(Encoding.UTF8.GetBytes(password));
            Tlv106Key = new Md5Cryptor().Encrypt(PasswordMd5
                        .Concat(new byte[] { 0x00, 0x00, 0x00, 0x00 })
                        .Concat(BitConverter.GetBytes(UinInfo.Uin).Reverse()).ToArray());

            DSecret = MakeDpassword();
            GSecret = MakeGSecret(imei, DSecret, null);

            SyncCookie = MakeSyncCookie();
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
            return ProtoTreeRoot.Serialize(new SyncCookie(DateTimeOffset.UtcNow.ToUnixTimeSeconds())).GetBytes();
        }
    }
}
