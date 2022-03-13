using System.Collections.Generic;
using Konata.Core.Packets;
using Konata.Core.Utils.Ecdh;
using Konata.Core.Utils.IO;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Konata.Core.Utils.Crypto;

internal class EcdhCryptor : ICryptor
{
    /// <summary>
    /// Ecdh cipher
    /// </summary>
    public ECDiffieHellman Ecdh { get; }

    private TeaCryptor Cryptor { get; }

    /// <summary>
    /// Ecdh curve
    /// </summary>
    public EllipticCurve Curve
        => Ecdh.Curve;

    /// <summary>
    /// Share key
    /// </summary>
    public byte[] ShareKey { get; private set; }

    /// <summary>
    /// Public key
    /// </summary>
    public byte[] PublicKey
        => Ecdh.GetPublicKeyPacked();

    /// <summary>
    /// Encrypt method
    /// </summary>
    public CryptMethod Method { get; }

    /// <summary>
    /// Encrypt method id
    /// </summary>
    public CryptId MethodId { get; }
    
    // Enctypt method
    public enum CryptMethod : uint
    {
        SecP192k1 = 0x0101 << 16 | 0x0102,
        Prime256v1 = 0x0201 << 16 | 0x0131
    }

    public enum CryptId
    {
        ECDH7 = 0x07,
        ECDH135 = 0x87,
    }

    private static readonly Dictionary<CryptMethod,
        (EllipticCurve Curve, CryptId Id, byte[] Pub)> _curveTable = new()
    {
        {
            // Server crypt method (Curve SecP192k1)
            CryptMethod.SecP192k1, (EllipticCurve.SecP192k1, CryptId.ECDH7, new byte[]
            {
                0x04, 0x92, 0x8D, 0x88, 0x50, 0x67, 0x30, 0x88,
                0xB3, 0x43, 0x26, 0x4E, 0x0C, 0x6B, 0xAC, 0xB8,
                0x49, 0x6D, 0x69, 0x77, 0x99, 0xF3, 0x72, 0x11,
                0xDE, 0xB2, 0x5B, 0xB7, 0x39, 0x06, 0xCB, 0x08,
                0x9F, 0xEA, 0x96, 0x39, 0xB4, 0xE0, 0x26, 0x04,
                0x98, 0xB5, 0x1A, 0x99, 0x2D, 0x50, 0x81, 0x3D,
                0xA8
            })
        },
        {
            // Server crypt method (Curve Prime256v1)
            CryptMethod.Prime256v1, (EllipticCurve.Prime256v1, CryptId.ECDH135, new byte[]
            {
                0x04, 0xEB, 0xCA, 0x94, 0xD7, 0x33, 0xE3, 0x99,
                0xB2, 0xDB, 0x96, 0xEA, 0xCD, 0xD3, 0xF6, 0x9A,
                0x8B, 0xB0, 0xF7, 0x42, 0x24, 0xE2, 0xB4, 0x4E,
                0x33, 0x57, 0x81, 0x22, 0x11, 0xD2, 0xE6, 0x2E,
                0xFB, 0xC9, 0x1B, 0xB5, 0x53, 0x09, 0x8E, 0x25,
                0xE3, 0x3A, 0x79, 0x9A, 0xDC, 0x7F, 0x76, 0xFE,
                0xB2, 0x08, 0xDA, 0x7C, 0x65, 0x22, 0xCD, 0xB0,
                0x71, 0x9A, 0x30, 0x51, 0x80, 0xCC, 0x54, 0xA8,
                0x2E
            })
        }
    };

    /// <summary>
    /// Create cryptor
    /// </summary>
    /// <param name="method"></param>
    public EcdhCryptor(CryptMethod method)
    {
        Method = method;
        Cryptor = TeaCryptor.Instance;

        // Select the curve
        var crypt = _curveTable[method];
        {
            Ecdh = new(crypt.Curve);
            MethodId = crypt.Id;
            ShareKey = GenerateShared(crypt.Pub);
        }
    }

    /// <summary>
    /// Key exchange
    /// </summary>
    /// <param name="bobPublic"></param>
    /// <returns></returns>
    public byte[] GenerateShared(byte[] bobPublic)
    {
        var unpack = Ecdh.UnpackPublic(bobPublic);
        ShareKey = Ecdh.KeyExchange(unpack);

        return ShareKey;
    }

    public byte[] Encrypt(byte[] data, byte[] randkey)
    {
        var buffer = new PacketBase();
        {
            buffer.PutUshortBE((ushort) ((uint) Method >> 16));
            buffer.PutBytes(randkey);
            buffer.PutUshortBE((ushort) ((uint) Method & 0xFFFF));

            if (Method == CryptMethod.Prime256v1)
                buffer.PutUshortBE(0x01);

            buffer.PutBytes(PublicKey, ByteBuffer.Prefix.Uint16);
            buffer.PutEncryptedBytes(data, Cryptor, ShareKey);
        }

        return buffer.GetBytes();
    }

    public byte[] Decrypt(byte[] data)
        => Cryptor.Decrypt(data, ShareKey);

    public byte[] Decrypt(byte[] data, byte[] key)
        => Decrypt(data);
}
