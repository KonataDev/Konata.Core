using NUnit.Framework;
using Konata.Core.Utils.Ecdh;

namespace Konata.Core.Test.UtilTest;

public class EcdhTest
{
    [Test]
    public void TestSecP192K1()
    {
        var aliceEcdh = new ECDiffieHellman(EllipticCurve.SecP192k1);
        var bobEcdh = new ECDiffieHellman(EllipticCurve.SecP192k1);
        {
            var aliceShare = aliceEcdh.KeyExchange(bobEcdh.GetPublicKey());
            var bobShare = bobEcdh.KeyExchange(aliceEcdh.GetPublicKey());
            Assert.AreEqual(aliceShare, bobShare);
        }
    }

    [Test]
    public void TestPrime256V1_1()
    {
        var aliceEcdh = new ECDiffieHellman(EllipticCurve.Prime256v1);
        var bobEcdh = new ECDiffieHellman(EllipticCurve.Prime256v1);
        {
            var aliceShare = aliceEcdh.KeyExchange(bobEcdh.GetPublicKey());
            var bobShare = bobEcdh.KeyExchange(aliceEcdh.GetPublicKey());
            Assert.AreEqual(aliceShare, bobShare);
        }
    }

    [Test]
    public void TestPublicUnpack()
    {
        var ecdh = new ECDiffieHellman(EllipticCurve.SecP192k1);
        var pubComp = ecdh.UnpackPublic(ecdh.GetPublicKeyPacked(true));
        var pubUncomp = ecdh.UnpackPublic(ecdh.GetPublicKeyPacked());
        {
            if (pubComp.X == pubUncomp.X && pubComp.Y == pubUncomp.Y) Assert.Pass();
            Assert.Fail();
        }
    }
}
