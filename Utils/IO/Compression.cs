using System.IO;
using System.Text;
using Ionic.Zlib;

// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Utils.IO
{
    public static class Compression
    {
        /// <summary>
        /// Inflate
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            using var ms = new MemoryStream();
            using var ds = new DeflateStream(ms, CompressionMode.Decompress, true);
            using var os = new MemoryStream();
        
            // We must ignore the magic header(first 2 bytes)
            // This is a HOLY rabbit hole that I fell into.
            ms.Write(data, 2, data.Length - 2);
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
        /// Zlib decompress
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ZDecompress(byte[] data)
        {
            using var os = new MemoryStream();
            using var zs = new ZlibStream(os, CompressionMode.Decompress);
        
            zs.Write(data, 0, data.Length);
            zs.Close();
        
            return os.ToArray();
        }
        
        /// <summary>
        /// Zlib compress
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ZCompress(byte[] data)
        {
            using var os = new MemoryStream();
            using var zs = new ZlibStream(os, CompressionMode.Compress);

            zs.Write(data, 0, data.Length);
            zs.Close();

            return os.ToArray();
        }

        /// <summary>
        /// Zlib compress
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ZCompress(string data)
            => ZCompress(Encoding.UTF8.GetBytes(data));
    }
}
