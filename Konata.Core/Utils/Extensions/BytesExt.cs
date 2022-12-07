using System.Runtime.CompilerServices;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.IO;

namespace Konata.Core.Utils.Extensions;

internal static class BytesExt
{
    /// <summary>
    /// Convert bytes to hex string
    /// </summary>
    /// <param name="data"></param>
    /// <param name="space"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHex(this byte[] data, bool space = false)
        => ByteConverter.Hex(data, space);

    /// <summary>
    /// Convert bytes to base64 string
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Btoa(this byte[] data)
        => ByteConverter.Base64(data);

    /// <summary>
    /// Calculate Adler32 checksum<br/>
    /// For detailed documentation, please ref
    /// <a herf="https://en.wikipedia.org/wiki/Adler-32"></a><br/>
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Adler32(this byte[] data)
    {
        uint a = 1, b = 0;
        for (long i = 0; i < data.Length; ++i)
        {
            a = (a + data[i]) % 65521;
            b = (b + a) % 65521;
        }

        return ByteConverter.UInt32ToBytes((b << 16) | a, Endian.Big);
    }

    /// <summary>
    /// Calculate Crc32 checksum
    /// For detailed documentation, please ref
    /// <a herf="https://en.wikipedia.org/wiki/Cyclic_redundancy_check"></a><br/>
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Crc32(this byte[] data)
    {
        var crc = 0xFFFFFFFFU;
        for (long i = 0; i < data.Length; ++i)
        {
            var ch = data[i];
            for (long j = 0; j < 8; ++j)
            {
                var b = (ch ^ crc) & 1;
                crc >>= 1;

                if (b != 0) crc ^= 0xEDB88320;
                ch >>= 1;
            }
        }

        return ByteConverter.UInt32ToBytes(~crc, Endian.Little);
    }

    /// <summary>
    /// Md5
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Md5(this byte[] data)
        => new Md5Cryptor().Encrypt(data);
}
