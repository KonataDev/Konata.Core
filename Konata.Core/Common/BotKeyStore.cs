using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.Ecdh;
using Konata.Core.Utils.Protobuf;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Common;

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
    /// Ecdh
    /// </summary>
    internal EcdhCryptor Ecdh { get; private set; }

    /// <summary>
    /// Fixed keys
    /// </summary>
    internal KeyStub KeyStub { get; }

    /// <summary>
    /// Create a key store
    /// </summary>
    public BotKeyStore()
    {
        Ecdh = new(EcdhCryptor.CryptMethod.Prime256v1);
        KeyStub = new KeyStub();
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
            PasswordMd5 = passwordMd5
        };

        Session = new WtLogin
        {
            DSecret = dSecret,
            Tlv106Key = new Md5Cryptor().Encrypt(passwordMd5
                .Concat(new byte[] {0x00, 0x00, 0x00, 0x00})
                .Concat(BitConverter.GetBytes(uinNum).Reverse()).ToArray())
        };

        KeyStub = new KeyStub
        {
            TgtgKey = MakeRandKey(16)
        };
    }

    internal void Initial(string imei)
    {
        Session.GSecret ??= MakeGSecret(imei, Session.DSecret, null);

        // Make sync cookie for syncing message
        Account.SyncCookie = MakeSyncCookie();

        // Roll a randkey
        KeyStub.RandKey = MakeRandKey(16);

        // Create cryptor
        Ecdh = new(EcdhCryptor.CryptMethod.Prime256v1);
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

    /// <summary>
    /// Make sync cookie
    /// </summary>
    /// <returns></returns>
    private static byte[] MakeSyncCookie()
    {
        return ProtoTreeRoot.Serialize(new SyncCookie
            (DateTimeOffset.UtcNow.ToUnixTimeSeconds())).GetBytes();
    }

    /// <summary>
    /// Make random key
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private static byte[] MakeRandKey(int length)
    {
        var buffer = new byte[length];
        RandomNumberGenerator.Fill(buffer);
        return buffer;
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
    /// 
    /// </summary>
    internal byte[] SyncCookie { get; set; }
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
    public ServerInfo Server { get; set; }

    public byte[] Ticket { get; set; }
}

internal class KeyStub
{
    internal byte[] ZeroKey { get; } =
    {
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
    };

    public byte[] TgtgKey { get; set; } =
    {
        0x2E, 0x39, 0x9A, 0x9C, 0xF2, 0x57, 0x12, 0xF8,
        0x1E, 0x5B, 0x63, 0x2E, 0xB3, 0xB3, 0xF7, 0x9F
    };

    /// <summary>
    /// Rand key
    /// </summary>
    public byte[] RandKey { get; set; } =
    {
        0xE2, 0xED, 0x53, 0x77, 0xAD, 0xFD, 0x99, 0x83,
        0x56, 0xEB, 0x8B, 0x4C, 0x62, 0x7C, 0x22, 0xC4
    };

    // /// <summary>
    // /// Default share key
    // /// </summary>
    // internal byte[] ShareKey { get; set; } =
    // {
    //     0x4D, 0xA0, 0xF6, 0x14, 0xFC, 0x9F, 0x29, 0xC2,
    //     0x05, 0x4C, 0x77, 0x04, 0x8A, 0x65, 0x66, 0xD7
    // };
    //
    // /// <summary>
    // /// Default public key
    // /// </summary>
    // internal byte[] PublicKey { get; set; } =
    // {
    //     0x02, 0x0B, 0x03, 0xCF, 0x3D, 0x99, 0x54, 0x1F,
    //     0x29, 0xFF, 0xEC, 0x28, 0x1B, 0xEB, 0xBD, 0x4E,
    //     0xA2, 0x11, 0x29, 0x2A, 0xC1, 0xF5, 0x3D, 0x71,
    //     0x28
    // };
}
