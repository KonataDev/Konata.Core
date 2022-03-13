using System;
using Konata.Core.Utils.Ecdh;
using Konata.Core.Utils.Extensions;
using NUnit.Framework;

namespace Konata.Core.Test.UtilTest;

public class EcdhTest
{
    [Test]
    public void TestSecP192K1()
    {
        var ecdh = new ECDiffieHellman(EllipticCurve.SecP192k1);
        var spub = ecdh.UnpackPublic("04928D8850673088B343264E0C6BACB8496D697799F37211DEB25BB73906CB089FEA9639B4E0260498B51A992D50813DA8".UnHex());
        {
            var sharedPt = ecdh.KeyExchange(spub);
            var publicPt = ecdh.GetPublicKeyPacked();

            Console.WriteLine(ecdh.GetSecret());
            Console.WriteLine(sharedPt.ToHex());
            Console.WriteLine(publicPt.ToHex());
            Assert.Pass();
        }
    }

    [Test]
    public void TestPrime256V1_1()
    {
        var ecdh = new ECDiffieHellman(EllipticCurve.Prime256v1);
        var spub = ecdh.UnpackPublic("04EBCA94D733E399B2DB96EACDD3F69A8BB0F74224E2B44E3357812211D2E62EFBC91BB553098E25E33A799ADC7F76FEB208DA7C6522CDB0719A305180CC54A82E".UnHex());
        {
            var sharedPt = ecdh.KeyExchange(spub);
            var publicPt = ecdh.GetPublicKeyPacked();

            Console.WriteLine(ecdh.GetSecret());
            Console.WriteLine(sharedPt.ToHex());
            Console.WriteLine(publicPt.ToHex());
            Assert.Pass();
        }
    }

    [Test]
    public void TestPrime256V1_2()
    {
        var ecdh = new ECDiffieHellman(EllipticCurve.Prime256v1);
        var spub = ecdh.UnpackPublic("0440EAF325B9C66225143AA7F3961C953C3D5A8048C2B73293CDC7DCBAB7F35C4C66AA8917A8FD511F9D969D02C8501BCAA3E3B11746F00567E3AEA303AC5F2D25".UnHex());
        {
            var sharedPt = ecdh.KeyExchange(spub);
            var publicPt = ecdh.GetPublicKeyPacked();

            Console.WriteLine(ecdh.GetSecret());
            Console.WriteLine(sharedPt.ToHex());
            Console.WriteLine(publicPt.ToHex());
            Assert.Pass();
        }
    }

    [Test]
    public void TestPublicUnpack()
    {
        var ecdh = new ECDiffieHellman(EllipticCurve.SecP192k1);
        var spubUncomp = ecdh.UnpackPublic("04D5CFB02D5D4FCA2C84F6F1294B455BAB4C9698DD572BF86382A9DAF8ADE9D45A57DE1404FA5D41291E0A56CB4508D32F".UnHex());
        var spubComp = ecdh.UnpackPublic("03D5CFB02D5D4FCA2C84F6F1294B455BAB4C9698DD572BF863".UnHex());
        {
            if (spubComp.X == spubUncomp.X && spubComp.Y == spubUncomp.Y) Assert.Pass();
            Assert.Fail();
        }
    }
}
