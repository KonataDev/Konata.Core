using System;

namespace Konata.Library.Protobuf
{
    internal static class VariantConv
    {
        public static long VariantToNumber(byte[] variant)
        {
            var number = 0L;

            for (int i = variant.Length; i > 0; --i)
            {
                number |= (variant[i] & 127) << (i * 7);
            }

            return number;
        }

        public static byte[] NumberToVariant(long number)
        {
            byte[] buffer;

            if (number >= 127)
            {
                var len = 0;
                buffer = new byte[10];

                do
                {
                    buffer[len] = (byte)((number & 127) | 128);
                    number >>= 7;
                    ++len;
                } while (number > 127);

                buffer[len] = (byte)number;
                Array.Resize(ref buffer, len + 1);
            }
            else
            {
                buffer = new byte[1] { (byte)number };
            }

            return buffer;
        }
    }
}
