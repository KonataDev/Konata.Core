using System;
using System.Numerics;
using System.Security.Cryptography;

// ReSharper disable UnusedType.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Konata.Core.Utils.Ecdh;

/// <summary>
/// 椭圆曲线 Diffie-Hellman (ECDH) 算法
/// Reference: https://asecuritysite.com/encryption/ecdh3
/// </summary>
internal class ECDiffieHellman
{
    public EllipticCurve Curve { get; set; }

    private static readonly Random random = new();
    private static readonly MD5 md5 = MD5.Create();

    public ECDiffieHellman(EllipticCurve curve)
        => Curve = curve;

    /// <summary>
    /// Generate the sec
    /// </summary>
    /// <returns></returns>
    public BigInteger CreateSecret()
    {
        BigInteger result;
        var array = new byte[Curve.Size + 1];

        do
        {
            random.NextBytes(array);
            array[Curve.Size] = 0;
            result = new BigInteger(array);
        } while (result < 1 || result >= Curve.N);

        return result;
    }

    /// <summary>
    /// 使用给予的私钥生成公钥
    /// </summary>
    public Point CreatePublic(BigInteger secret)
        => CreateShared(secret, Curve.G);

    /// <summary>
    /// Calculate shared based on pub and sec
    /// </summary>
    /// <param name="secret"></param>
    /// <param name="public"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Point CreateShared(BigInteger secret, Point @public)
    {
        if (secret % Curve.N == 0 || @public.IsDefault) return default;
        if (secret < 0) return CreateShared(-secret, -@public);

        if (!Curve.CheckOn(@public))
            throw new Exception("Public key does not correct.");

        Point pr = default;
        Point pa = @public;
        while (secret > 0)
        {
            if ((secret & 1) > 0)
                pr = PointAdd(pr, pa);

            pa = PointAdd(pa, pa);
            secret >>= 1;
        }

        if (!Curve.CheckOn(pr))
            throw new Exception("Unknown error.");

        return pr;
    }

    /// <summary>
    /// Pack secret
    /// </summary>
    /// <param name="secret"></param>
    /// <returns></returns>
    public byte[] PackSecret(BigInteger secret)
    {
        byte[] result = secret.ToByteArray();
        int length = result.Length;
        Array.Resize(ref result, length + 4);
        Array.Reverse(result);
        result[3] = (byte) length;
        return result;
    }

    /// <summary>
    /// Pack public
    /// </summary>
    /// <param name="public"></param>
    /// <param name="compress"></param>
    /// <returns></returns>
    public byte[] PackPublic(Point @public, bool compress = true)
    {
        if (compress)
        {
            var result = @public.X.ToByteArray();
            if (result.Length == Curve.Size)
                Array.Resize(ref result, Curve.Size + 1);

            Array.Reverse(result);
            result[0] = (byte) ((@public.Y.IsEven ^ @public.Y.Sign < 0) ? 0x02 : 0x03);
            return result;
        }

        var x = TakeReverse(@public.X.ToByteArray(), Curve.Size);
        var y = TakeReverse(@public.Y.ToByteArray(), Curve.Size);
        var buffer = new byte [Curve.Size * 2 + 1];
        {
            buffer[0] = 0x04;
            Buffer.BlockCopy(x, 0, buffer, 1, x.Length);
            Buffer.BlockCopy(y, 0, buffer, y.Length + 1, x.Length);
        }
        return buffer;
    }

    /// <summary>
    /// 使用MD5对共享密钥打包
    /// </summary>
    public byte[] PackShared(Point shared)
    {
        var x = TakeReverse(shared.X.ToByteArray(), Curve.Size);
        return md5.ComputeHash(x[..Curve.PackSize]);
    }

    /// <summary>
    /// 私钥解包
    /// </summary>
    public BigInteger UnpackSecret(byte[] secret)
    {
        var length = secret.Length - 4;
        if (length != secret[3])
            throw new Exception("Length does not match.");

        // Teardown data
        var temp = new byte[length];
        Buffer.BlockCopy(secret, 4, temp, 0, length);

        return new(temp);
    }

    /// <summary>
    /// Decompress public key
    /// </summary>
    public Point UnpackPublic(byte[] @public)
    {
        var length = @public.Length;
        if (length != Curve.Size * 2 + 1)
            throw new Exception("Length does not match.");
        if (@public[0] != 0x04)
            throw new Exception("Not supported packed public key.");

        // Teardown x and y
        var x = new byte[Curve.Size];
        var y = new byte[Curve.Size];

        Buffer.BlockCopy(@public, 1, x, 0, Curve.Size);
        Buffer.BlockCopy(@public, Curve.Size + 1, y, 0, Curve.Size);
        {
            // To LE
            Array.Reverse(x);
            Array.Reverse(y);

            // Append 0x00
            Array.Resize(ref x, x.Length + 1);
            Array.Resize(ref y, y.Length + 1);
        }

        return new Point(new(x), new(y));
    }

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
        if (a < 0)
        {
            return p - ModInverse(-a, p);
        }

        BigInteger g = BigInteger.GreatestCommonDivisor(a, p);
        if (g != 1)
        {
            throw new Exception("Inverse does not exist.");
        }

        return BigInteger.ModPow(a, p - 2, p);
    }

    private Point PointAdd(Point p1, Point p2)
    {
        if (p1.IsDefault) return p2;
        if (p2.IsDefault) return p1;

        if (!Curve.CheckOn(p1) || !Curve.CheckOn(p2))
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
                m = (3 * x1 * x1 + Curve.A) * ModInverse(y1 << 1, Curve.P);
            }
            else
            {
                return default;
            }
        }
        else
        {
            m = (y1 - y2) * ModInverse(x1 - x2, Curve.P);
        }

        BigInteger xr = (m * m - x1 - x2) % Curve.P;
        Point pr = new Point(xr, (m * (x1 - xr) - y1) % Curve.P);
        if (!Curve.CheckOn(pr))
        {
            throw new Exception();
        }

        return pr;
    }
}
