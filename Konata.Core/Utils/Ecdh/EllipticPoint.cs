﻿using System.Numerics;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Konata.Core.Utils.Ecdh;

internal struct EllipticPoint
{
    public BigInteger X { get; set; }

    public BigInteger Y { get; set; }

    public EllipticPoint(BigInteger x, BigInteger y)
    {
        X = x;
        Y = y;
    }

    public bool IsDefault => X == 0 && Y == 0;

    public override string ToString() => $"({X:X}, {Y:X})";

    public static EllipticPoint operator -(EllipticPoint p) => new(-p.X, -p.Y);
}
