using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace Konata.Utils.IO
{
    public static class Deflate
    {
        public static byte[] Decompress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress, true))
            using (MemoryStream os = new MemoryStream())
            {
                // We must ignore the magic header
                // This is a HOLY rabbit hole that I fell into.
                ms.Position = 2;

                // Decompress all data
                ds.CopyTo(os);

                return os.ToArray();
            }
        }
    }
}
