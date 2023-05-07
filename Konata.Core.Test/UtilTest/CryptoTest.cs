using System;
using System.Globalization;
using NUnit.Framework;
using Konata.Core.Utils.TencentEncrypt;

namespace Konata.Core.Test.UtilTest;

public class CryptoTest
{
    [Test]
    public void TestT544()
    {
        var test = UnHex("0c05d28b405bce1595c70ffa694ff163d4b600f229482e07de32c8000000003525382c00000000");
        var r = Algorithm.Sign(0, Array.Empty<byte>());
        
        Assert.AreEqual(test[0], r[0]);
        Assert.AreEqual(test[1], r[1]);
        
        Assert.AreEqual(test[35..39], r[35..39]);
    }
    
    private static byte[] UnHex(string hex)
    {
        var result = new byte[hex.Length / 2];
        for (int i = 0; i < hex.Length; i += 2) result[i / 2] = byte.Parse(hex.Substring(i, 2), NumberStyles.HexNumber);
        return result;
    }
}