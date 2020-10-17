using System;

namespace Konata.Library.Protobuf
{
    internal static class VariantConv
    {
        public static void VariantToNumber()
        {

        }

        public static byte[] NumberToVariant(long value)
        {
            byte[] buffer;

            if (value >= 127)
            {
                var len = 0;
                buffer = new byte[10];

                do
                {
                    buffer[len] = (byte)((value & 127) | 128);
                    value >>= 7;
                    ++len;
                } while (value > 127);

                buffer[len] = (byte)value;
                Array.Resize(ref buffer, len + 1);
            }
            else
            {
                buffer = new byte[1] { (byte)value };
            }

            return buffer;
        }
    }
}
