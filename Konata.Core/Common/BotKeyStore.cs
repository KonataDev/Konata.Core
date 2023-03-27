using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.Protobuf;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable ClassNeverInstantiated.Global
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

    internal void Initial(BotConfig config, BotDevice device)
    {
        Session.GSecret ??= MakeGSecret(device.Model.Imei, Session.DSecret, null);

        // Make sync cookie for syncing message
        var cookie = MakeSyncCookie();
        Account.SyncCookie = cookie.Cookie;
        Account.SyncCookieConsts = cookie.Consts;

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
            RandomNumberGenerator.Fill(seedTable);

            for (var i = 0; i < seedTable.Length; ++i)
            {
                seedTable[i] = (byte) (Math.Abs(seedTable[i] % 26) 
                                       + (random.Next(0, 1) == 1 ? 97 : 65));
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
    private static ((uint, uint) Consts, byte[] Cookie) MakeSyncCookie()
    {
        // Make constants
        var seeds = MakeRandomSeeds();
        var random = new Random(seeds);

        var const1 = (uint) random.Next(int.MinValue, int.MaxValue);
        var const2 = (uint) random.Next(int.MinValue, int.MaxValue);
        var cookie = ProtoTreeRoot.Serialize(new SyncCookie((const1, const2))).GetBytes();
        return ((const1, const2), cookie);
    }

    private static int MakeRandomSeeds()
        => new Random().Next();

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
    /// Sync cookie
    /// </summary>
    internal byte[] SyncCookie { get; set; }

    /// <summary>
    /// Sync cookie const
    /// </summary>
    internal (uint, uint) SyncCookieConsts { get; set; }
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

    /// <summary>
    /// T547 data for submiting slider captcha
    /// </summary>
    internal byte[] WtSessionT547 { get; set; }
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
}
