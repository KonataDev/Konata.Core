using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Konata.Library.Ecdh.ECDHAlgorithm
{
    using bint = BigInteger;

    public struct Point
    {
        public bint X { get; set; }

        public bint Y { get; set; }

        public Point(bint x, bint y)
        {
            X = x;
            Y = y;
        }

        public bool IsDefault => X == 0 && Y == 0;

        public override string ToString() => $"({X:X}, {Y:X})";

        public static Point operator -(Point p) => new Point(-p.X, -p.Y);
    }
}