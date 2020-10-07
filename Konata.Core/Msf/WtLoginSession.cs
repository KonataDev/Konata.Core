using System;
using System.Text;
using System.Security.Cryptography;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf
{
    class WtLoginSession
    {
        public byte[] _gSecret;
        public string _dPassword;

        public string _smsToken;
        public string _smsPhone;

        public string _sigSession;

        public WtLoginSession()
        {
            _dPassword = MakeDpassword();
            _gSecret = MakeGSecret(DeviceInfo.System.Imei, _dPassword, null);
        }

        public static byte[] MakeGSecret(string imei, string dpwd, byte[] salt)
        {
            var imeiByte = Encoding.UTF8.GetBytes(imei);
            var dpwdByte = Encoding.UTF8.GetBytes(dpwd);

            var buffer = new byte[imeiByte.Length + dpwdByte.Length +
                (salt != null ? salt.Length : 0)];
            return new Md5Cryptor().Encrypt(buffer);
        }

        public static string MakeDpassword()
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
    }
}
