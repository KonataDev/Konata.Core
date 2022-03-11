using System.Numerics;

namespace Konata.Core.Utils.Ecdh;

internal struct Point
{
    public BigInteger X { get; set; }

    public BigInteger Y { get; set; }

    public Point(BigInteger x, BigInteger y)
    {
        X = x;
        Y = y;
    }

    public bool IsDefault => X == 0 && Y == 0;

    public override string ToString() => $"({X:X}, {Y:X})";

    public static Point operator -(Point p) => new Point(-p.X, -p.Y);
}
