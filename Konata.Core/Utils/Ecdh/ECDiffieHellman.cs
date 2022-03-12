using System;
using System.Numerics;
using System.Security.Cryptography;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Konata.Core.Utils.Ecdh;

/// <summary>
/// 椭圆曲线 Diffie-Hellman (ECDH) 算法
/// Reference: https://asecuritysite.com/encryption/ecdh3
/// </summary>
internal class ECDiffieHellman
{
    /// <summary>
    /// Curve parameters
    /// </summary>
    public EllipticCurve Curve { get; }

    /// <summary>
    /// Secret
    /// </summary>
    private BigInteger Secret { get; set; }

    /// <summary>
    /// Public
    /// </summary>
    private EllipticPoint Public { get; set; }

    public ECDiffieHellman(EllipticCurve curve)
    {
        Curve = curve;
        Secret = CreateSecret();
        Public = CreatePublic(Secret);
    }

    /// <summary>
    /// Pack public key
    /// </summary>
    /// <param name="compress"></param>
    /// <returns></returns>
    public byte[] GetPublicKeyPacked(bool compress = false)
        => PackPublic(Public, compress);

    /// <summary>
    /// Pack secret
    /// </summary>
    /// <returns></returns>
    public byte[] GetSecretPacked()
        => PackSecret(Secret);

    /// <summary>
    /// Get public key
    /// </summary>
    /// <returns></returns>
    public EllipticPoint GetPublicKey() => Public;

    /// <summary>
    /// Get secret
    /// </summary>
    /// <returns></returns>
    public BigInteger GetSecret() => Secret;

    /// <summary>
    /// Set new secret
    /// </summary>
    /// <param name="secret"></param>
    public void SetSecret(BigInteger secret)
    {
        Secret = secret;
        Public = CreatePublic(Secret);
    }

    /// <summary>
    /// Key exchange with bob
    /// </summary>
    /// <param name="ecpub"></param>
    public byte[] KeyExchange(EllipticPoint ecpub)
    {
        var shared = CreateShared(Secret, ecpub);
        return PackShared(shared);
    }

    /// <summary>
    /// Decompress public key
    /// </summary>
    public EllipticPoint UnpackPublic(byte[] publicKey)
    {
        var length = publicKey.Length;
        if (length != Curve.Size * 2 + 1)
            throw new Exception("Length does not match.");
        if (publicKey[0] != 0x04)
            throw new Exception("Not supported packed public key.");

        // Teardown x and y
        var x = new byte[Curve.Size];
        var y = new byte[Curve.Size];

        Buffer.BlockCopy(publicKey, 1, x, 0, Curve.Size);
        Buffer.BlockCopy(publicKey, Curve.Size + 1, y, 0, Curve.Size);
        {
            // To LE
            Array.Reverse(x);
            Array.Reverse(y);

            // Append 0x00
            Array.Resize(ref x, x.Length + 1);
            Array.Resize(ref y, y.Length + 1);
        }

        return new EllipticPoint(new(x), new(y));
    }

    /// <summary>
    /// Pack public
    /// </summary>
    /// <param name="ecpub"></param>
    /// <param name="compress"></param>
    /// <returns></returns>
    public byte[] PackPublic(EllipticPoint ecpub, bool compress = true)
    {
        if (compress)
        {
            var result = ecpub.X.ToByteArray();
            if (result.Length == Curve.Size)
                Array.Resize(ref result, Curve.Size + 1);

            Array.Reverse(result);
            result[0] = (byte) ((ecpub.Y.IsEven ^ ecpub.Y.Sign < 0) ? 0x02 : 0x03);
            return result;
        }

        var x = TakeReverse(ecpub.X.ToByteArray(), Curve.Size);
        var y = TakeReverse(ecpub.Y.ToByteArray(), Curve.Size);
        var buffer = new byte [Curve.Size * 2 + 1];
        {
            buffer[0] = 0x04;
            Buffer.BlockCopy(x, 0, buffer, 1, x.Length);
            Buffer.BlockCopy(y, 0, buffer, y.Length + 1, x.Length);
        }

        return buffer;
    }

    /// <summary>
    /// Pack secret
    /// </summary>
    /// <param name="ecsec"></param>
    /// <returns></returns>
    private byte[] PackSecret(BigInteger ecsec)
    {
        var result = ecsec.ToByteArray();
        var length = result.Length;
        {
            Array.Resize(ref result, length + 4);
            Array.Reverse(result);

            result[3] = (byte) length;
        }

        return result;
    }

    /// <summary>
    /// 使用MD5对共享密钥打包
    /// </summary>
    private byte[] PackShared(EllipticPoint ecshared)
    {
        var x = TakeReverse(ecshared.X.ToByteArray(), Curve.Size);
        return MD5.Create().ComputeHash(x[..Curve.PackSize]);
    }

    /// <summary>
    /// Unpack secret
    /// </summary>
    private BigInteger UnpackSecret(byte[] ecsec)
    {
        var length = ecsec.Length - 4;
        if (length != ecsec[3])
            throw new Exception("Length does not match.");

        // Teardown data
        var temp = new byte[length];
        Buffer.BlockCopy(ecsec, 4, temp, 0, length);

        return new(temp);
    }

    /// <summary>
    /// Generate the secret from public
    /// </summary>
    private EllipticPoint CreatePublic(BigInteger ecsec)
        => CreateShared(ecsec, Curve.G);

    /// <summary>
    /// Generate the secret
    /// </summary>
    /// <returns></returns>
    private BigInteger CreateSecret()
    {
        BigInteger result;
        var array = new byte[Curve.Size + 1];

        do
        {
            // Roll a new secure recret
            RandomNumberGenerator.Fill(array);
            array[Curve.Size] = 0;

            // Convert to BigInt
            result = new BigInteger(array);
        } while (result < 1 || result >= Curve.N);

        return result;
    }

    /// <summary>
    /// Calculate shared based on public and secret
    /// </summary>
    /// <param name="ecsec"></param>
    /// <param name="ecpub"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private EllipticPoint CreateShared(BigInteger ecsec, EllipticPoint ecpub)
    {
        if (ecsec % Curve.N == 0 || ecpub.IsDefault) return default;
        if (ecsec < 0) return CreateShared(-ecsec, -ecpub);

        if (!Curve.CheckOn(ecpub))
            throw new Exception("Public key does not correct.");

        EllipticPoint pr = default;
        EllipticPoint pa = ecpub;
        while (ecsec > 0)
        {
            if ((ecsec & 1) > 0)
                pr = PointAdd(Curve, pr, pa);

            pa = PointAdd(Curve, pa, pa);
            ecsec >>= 1;
        }

        if (!Curve.CheckOn(pr))
            throw new Exception("Unknown error.");

        return pr;
    }

    /// <summary>
    /// Point add
    /// </summary>
    /// <param name="curve"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static EllipticPoint PointAdd(EllipticCurve curve, EllipticPoint p1, EllipticPoint p2)
    {
        if (p1.IsDefault) return p2;
        if (p2.IsDefault) return p1;

        if (!curve.CheckOn(p1) || !curve.CheckOn(p2))
            throw new Exception();

        BigInteger x1 = p1.X;
        BigInteger x2 = p2.X;
        BigInteger y1 = p1.Y;
        BigInteger y2 = p2.Y;
        BigInteger m;

        if (x1 == x2)
        {
            if (y1 == y2)
            {
                m = (3 * x1 * x1 + curve.A) * ModInverse(y1 << 1, curve.P);
            }
            else
            {
                return default;
            }
        }
        else
        {
            m = (y1 - y2) * ModInverse(x1 - x2, curve.P);
        }

        var xr = Mod(m * m - x1 - x2, curve.P);
        var yr = Mod(m * (x1 - xr) - y1, curve.P);
        var pr = new EllipticPoint(xr, yr);
        {
            if (!curve.CheckOn(pr))
                throw new Exception();
        }

        return pr;
    }

    #region Utils

    private static byte[] TakeReverse(byte[] array, int length)
    {
        var result = new byte[length];
        for (int i = 0, j = length - 1; i < length; ++i, --j)
        {
            result[i] = array[j];
        }

        return result;
    }

    /// <summary>
    /// Find modular inverse of <paramref name="a"/> under modulo <paramref name="p"/>.
    /// <paramref name="p"/> must be prime.
    /// Reference: https://www.geeksforgeeks.org/multiplicative-inverse-under-modulo-m/
    /// </summary>
    private static BigInteger ModInverse(BigInteger a, BigInteger p)
    {
        if (a < 0) return p - ModInverse(-a, p);

        BigInteger g = BigInteger.GreatestCommonDivisor(a, p);
        if (g != 1) throw new Exception("Inverse does not exist.");

        return BigInteger.ModPow(a, p - 2, p);
    }

    private static BigInteger Mod(BigInteger a, BigInteger b)
    {
        var result = a % b;
        if (result < 0) result += b;
        return result;
    }

    #endregion
}
