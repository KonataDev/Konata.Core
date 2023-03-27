using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using Konata.Core.Utils.IO;

namespace Konata.Core.Packets.Tlv.Model;

/// <summary>
/// Parse of T546 "Pow", migrated from mirai/cai
/// </summary>
internal class T546Body : TlvBody
{
    private readonly byte _version;
    private readonly int _algorithmType;
    private readonly byte _hashType;
    private readonly byte[] _source;
    private readonly byte[] _target; // tgt for client
    private readonly byte[] _clientPrivateKey;
    
    public readonly byte[] Parsed;
    public readonly byte[] Destination;
    public readonly uint Count;
    public readonly uint Elapsed;

    /// <summary>
    /// Parser of T546 data to output the T547
    /// </summary>
    /// <param name="data"></param>
    public T546Body(byte[] data) : base(data)
    {
        TakeByte(out _version);
        TakeIntBE(out _algorithmType);
        TakeByte(out _hashType);
        
        EatBytes(5);

        var sourceLength = TakeShort(out _, Endian.Big);
        var tgtLength = TakeShort(out _, Endian.Big);
        var cpkLength = TakeShort(out _, Endian.Big);
        TakeBytes(out _source, (uint)sourceLength);
        TakeBytes(out _target, (uint)tgtLength);
        TakeBytes(out _clientPrivateKey, (uint)cpkLength);

        if (_target.Length != 32) throw new InvalidDataException("tgt length should be 32, parse failed");
        
        long start = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // start = time.time()
        var tmpSrc = new BigInteger(_source, false, true); // tmpSrc = int.from_bytes(src, "big", signed=False) // should use BitIntegers as it has 128 bytes
        var hashCurrent = Hasher(tmpSrc);

        while (!hashCurrent.SequenceEqual(_target))
        {
            tmpSrc += 1;
            hashCurrent = Hasher(tmpSrc);
            Count += 1;
        }
        
        Destination = tmpSrc.ToByteArray(false, true);
        Elapsed = (uint)((DateTimeOffset.UtcNow.ToUnixTimeSeconds() - start) * 1000); // elp = time.time() - start

        Parsed = data;
        Parsed[3] = 0x01;

        byte[] Hasher(BigInteger source)
        {
            using var sha256 = SHA256.Create();
            var sourceBytes = source.ToByteArray(false, true);
            return sha256.ComputeHash(sourceBytes);
        }
    }
}