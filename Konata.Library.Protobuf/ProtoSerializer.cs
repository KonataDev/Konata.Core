using System;
using Konata.Library.IO;

namespace Konata.Library.Protobuf
{
    public static class ProtoSerializer
    {
        public static ByteBuffer Serialize(ProtoTreeRoot tree)
        {
            var buffer = new ByteBuffer();
            {
                foreach (var element in tree.leaves)
                {
                    var split = element.path.Split('.');
                    if (split.Length <= 0)
                        continue;

                    buffer.PutByte(Tag(split[split.Length - 1]));
                    if (element.needLength)
                        buffer.PutBytes(VariantConv.NumberToVariant(element.data.Length));
                    buffer.PutBytes(element.data);
                }
            }

            return buffer;
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
