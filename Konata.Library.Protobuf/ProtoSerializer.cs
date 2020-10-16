using System;
using Konata.Library.IO;

namespace Konata.Library.Protobuf
{
    public static class ProtoSerializer
    {
        public static byte[] Serialize(ProtoTreeRoot tree)
        {
            var buffer = new ByteBuffer();
            {
                foreach (var element in tree._leaves)
                {
                    var split = element._path.Split('.');
                    if (split.Length <= 0)
                        continue;

                    buffer.PutByte(Tag(split[split.Length - 1]));
                    buffer.PutBytes(element._data, 1);
                }
            }

            return buffer.GetBytes();
        }

        public static void DebugPrintTree(ProtoTreeRoot tree)
        {

        }

        private static byte Tag(string hex)
        {
            return (byte)((ByteConv(hex[0]) << 4) + ByteConv(hex[1]));
        }

        private static byte ByteConv(char ch)
        {
            if (ch >= '0' && ch <= '9')
            {
                return (byte)(ch - '0');
            }
            if (ch >= 'a' && ch <= 'f')
            {
                return (byte)((ch - 'a') + 10);
            }
            if (ch < 'A' || ch > 'F')
            {
                return 0;
            }
            return (byte)((ch - 'A') + 10);
        }
    }
}
