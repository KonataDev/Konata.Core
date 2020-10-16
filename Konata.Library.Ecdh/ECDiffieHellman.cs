using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Konata.Library.Ecdh.ECDHAlgorithm
{
    using bint = BigInteger;

    /// <summary>
    /// Reference: https://asecuritysite.com/encryption/ecdh3
    /// </summary>
    public class ECDiffieHellman
    {
        public EllipticCurve Curve { get; set; }

        public ECDiffieHellman(EllipticCurve curve) => Curve = curve;

        /// <summary>
        /// Generate random secret key.
        /// </summary>
        public bint CreateSecret()
        {
            byte[] array = new byte[Curve.Size + 1];
            bint result;
            do
            {
                random.NextBytes(array);
                array[Curve.Size] = 0;
                result = new bint(array);
            }
            while (result < 1 || result >= Curve.N);
            return result;
        }

        /// <summary>
        /// Generate public key with given secret key.
        /// </summary>
        public Point CreatePublic(bint secret) => CreateShared(secret, Curve.G);

        /// <summary>
        /// Calculate shared key with given secret key and public key.
        /// </summary>
        public Point CreateShared(bint secret, Point @public)
        {
            if (secret % Curve.N == 0 || @public.IsDefault)
            {
                return default;
            }
            if (secret < 0)
            {
                return CreateShared(-secret, -@public);
            }
            if (!Curve.CheckOn(@public))
            {
                throw new Exception("Public key does not correct.");
            }
            Point pr = default;
            Point pa = @public;
            while (secret > 0)
            {
                if ((secret & 1) > 0)
                {
                    pr = PointAdd(pr, pa);
                }
                pa = PointAdd(pa, pa);
                secret >>= 1;
            }
            if (!Curve.CheckOn(pr))
            {
                throw new Exception("Unknown error.");
            }
            return pr;
        }

        /// <summary>
        /// Pack secret key.
        /// </summary>
        public byte[] PackSecret(bint secret)
        {
            byte[] result = secret.ToByteArray();
            int length = result.Length;
            Array.Resize(ref result, length + 4);
            Array.Reverse(result);
            result[3] = (byte)length;
            return result;
        }

        /// <summary>
        /// Compress public key.
        /// </summary>
        public byte[] PackPublic(Point @public)
        {
            byte[] result = @public.X.ToByteArray();
            if (result.Length == Curve.Size)
            {
                Array.Resize(ref result, Curve.Size + 1);
            }
            Array.Reverse(result);
            result[0] = (byte)((@public.Y.IsEven ^ @public.Y.Sign < 0) ? 0x02 : 0x03);
            return result;
        }

        /// <summary>
        /// Compute hash of shared key with MD5.
        /// </summary>
        public byte[] PackShared(Point shared) => md5.ComputeHash(TakeReverse(shared.X.ToByteArray(), 24));

        /// <summary>
        /// Unpack secret key.
        /// </summary>
        public bint UnpackSecret(byte[] secret)
        {
            int length = secret.Length - 4;
            if (length != secret[3])
            {
                throw new Exception("Length does not match.");
            }
            byte[] temp = new byte[length];
            Buffer.BlockCopy(secret, 4, temp, 0, length);
            return new bint(temp);
        }

        /// <summary>
        /// [Not Implemented] Decompress public key.
        /// </summary>
        public Point UnpackPublic(byte[] @public)
        {
            throw new NotImplementedException();
        }

        private static readonly Random random = new Random();

        private static readonly MD5 md5 = MD5.Create();

        private static byte[] TakeReverse(byte[] array, int length)
        {
            byte[] result = new byte[length];
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
        private static bint ModInverse(bint a, bint p)
        {
            if (a < 0)
            {
                return p - ModInverse(-a, p);
            }
            bint g = bint.GreatestCommonDivisor(a, p);
            if (g != 1)
            {
                throw new Exception("Inverse does not exist.");
            }
            return bint.ModPow(a, p - 2, p);
        }

        private Point PointAdd(Point p1, Point p2)
        {
            if (p1.IsDefault)
            {
                return p2;
            }
            if (p2.IsDefault)
            {
                return p1;
            }
            if (!Curve.CheckOn(p1) || !Curve.CheckOn(p2))
            {
                throw new Exception();
            }
            bint x1 = p1.X;
            bint x2 = p2.X;
            bint y1 = p1.Y;
            bint y2 = p2.Y;
            bint m;
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
            bint xr = (m * m - x1 - x2) % Curve.P;
            Point pr = new Point(xr, (m * (x1 - xr) - y1) % Curve.P);
            if (!Curve.CheckOn(pr))
            {
                throw new Exception();
            }
            return pr;
        }
    }
}