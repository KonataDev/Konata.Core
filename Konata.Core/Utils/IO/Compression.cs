using System.IO;
using System.IO.Compression;
using System.Text;
using Konata.Core.Utils.Extensions;

// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Utils.IO;

internal static class Compression
{
    /// <summary>
    /// Deflate
    /// </summary>
    /// <param name="data"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static byte[] Deflate(byte[] data, CompressionLevel level)
    {
        using var os = new MemoryStream();
        using var ds = new DeflateStream(os, level, true);

        // Write data
        ds.Write(data, 0, data.Length);
        ds.Close();

        // Read the data
        var cmpdata = new byte[os.Length];
        {
            os.Position = 0;
            os.Read(cmpdata, 0, cmpdata.Length);
        }

        return cmpdata;
    }

    /// <summary>
    /// Inflate
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] Inflate(byte[] data)
    {
        using var ms = new MemoryStream();
        using var ds = new DeflateStream(ms, CompressionMode.Decompress, true);
        using var os = new MemoryStream();

        // Write data
        ms.Write(data, 0, data.Length);
        ms.Position = 0;

        // Decompress all data
        ds.CopyTo(os);
        var deflate = new byte[os.Length];
        {
            os.Position = 0;
            os.Read(deflate, 0, deflate.Length);
        }
        return deflate;
    }

    /// <summary>
    /// Zlib compress
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] ZCompress(byte[] data)
    {
        using var os = new MemoryStream();
        var deflate = Deflate(data, CompressionLevel.Optimal);
        {
            // Zlib header
            os.WriteByte(0x78); // Magic
            os.WriteByte(0xDA); // |

            // Write data
            os.Write(deflate);

            // Calculate adler32 checksum
            var checksum = data.Adler32();
            os.Write(checksum);
        }
        return os.ToArray();
    }

    /// <summary>
    /// Zlib compress
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] ZCompress(string data)
        => ZCompress(Encoding.UTF8.GetBytes(data));

    /// <summary>
    /// Zlib decompress
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] ZDecompress(byte[] data)
        => Inflate(data[2..^4]);

    /// <summary>
    /// Gzip compress
    /// For detailed documentation, please ref
    /// <a herf="https://docs.fileformat.com/compression/gz/#gz-file-header"></a><br/>
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] GZip(byte[] data)
    {
        using var os = new MemoryStream();
        var deflate = Deflate(data, CompressionLevel.Optimal);
        {
            // Gzip header
            os.WriteByte(0x1F); // Magic
            os.WriteByte(0x8B); // |
            os.WriteByte(0x08); // Compression Method * 0-7 (Reserved) * 8 (Deflate)
            os.WriteByte(0x00); // File Flags
            os.WriteByte(0x00); // 32-bit timestamp
            os.WriteByte(0x00); // |
            os.WriteByte(0x00); // |
            os.WriteByte(0x00); // |
            os.WriteByte(0x00); // Compression flags
            os.WriteByte(0x00); // Operating system ID

            // Write data
            os.Write(deflate);

            // Calculate crc32 checksum
            var checksum = data.Crc32();
            os.Write(checksum);

            // Write length
            os.Write(ByteConverter.UInt32ToBytes
                ((uint) data.Length, Endian.Little));
        }

        return os.ToArray();
    }

    /// <summary>
    /// Gunzip decompress
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] GunZip(byte[] data)
        => Inflate(data[10..^8]);
}
