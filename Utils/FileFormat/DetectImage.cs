using Konata.Core.Utils.IO;

namespace Konata.Core.Utils.FileFormat
{
    public static partial class FileFormat
    {
        //0 => ImageType.JPG,
        //1 => ImageType.PNG,
        //2 => ImageType.GIF,
        //3 => ImageType.BMP,
        //4 => ImageType.WEBP,

        /// <summary>
        /// Dectet image type
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static bool DetectImage(byte[] data,
            out int type, out uint width, out uint height)
        {
            var buffer = new ByteBuffer(data);
            {
                buffer.PeekUintBE(0, out var value);

                // ÿØ JFIF
                if (value >> 16 == 0xFFD8)
                {
                    type = 0;
                    return DetectJPG(buffer, out width, out height);
                }

                // ‰PNG
                if (value == 0x89504E47)
                {
                    type = 1;
                    return DetectPNG(buffer, out width, out height);
                }

                // GIF
                if (value >> 8 == 0x474946)
                {
                    type = 3;
                    return DetectGIF(buffer, out width, out height);
                }

                // BM
                if (value >> 16 == 0x424D)
                {
                    type = 4;
                    return DetectBMP(buffer, out width, out height);
                }

                // RIFF
                if (value == 0x52494646)
                {
                    // WEBP
                    buffer.PeekUintBE(8, out value);
                    if (value == 0x57454250)
                    {
                        type = 5;
                        return DetectWEBP(buffer, out width, out height);
                    }
                }
            }

            type = 0;
            width = 0;
            height = 0;
            return false;
        }

        private static bool DetectJPG(ByteBuffer buffer,
            out uint width, out uint height)
        {
            // FF D8
            // The Magic
            buffer.EatBytes(2);

            // Ignore tags
            ushort dataTag = 0x0000;
            while (buffer.RemainLength > 2)
            {
                // Take tag
                buffer.TakeUshortBE(out dataTag);
                if (dataTag is >= 0xFFC0 and <= 0xFFC3)
                {
                    break;
                }

                // Jump 1 byte
                if (dataTag == 0xFFFF)
                {
                    buffer.EatBytes(1);
                    continue;
                }

                // Take length
                buffer.TakeUshortBE(out var len);

                // Eat bytes
                buffer.EatBytes((uint) len - 2);
            }

            // Invalid JFIF file
            if (dataTag == 0x0000)
            {
                width = height = 0;
                return false;
            }

            // FF C0 baseline
            // FF C1 extended
            // FF C2 progressive
            // FF C3 lossless
            buffer.PeekUshortLE(0x03, out var h);
            height = h;
            buffer.PeekUshortLE(0x05, out var w);
            width = w;

            return true;
        }

        private static bool DetectPNG(ByteBuffer buffer,
            out uint width, out uint height)
        {
            buffer.PeekUintBE(0x10, out width);
            buffer.PeekUintBE(0x14, out height);
            return true;
        }

        private static bool DetectGIF(ByteBuffer buffer,
            out uint width, out uint height)
        {
            buffer.PeekUshortLE(0x06, out var w);
            width = w;
            buffer.PeekUshortLE(0x08, out var h);
            height = h;
            return true;
        }

        private static bool DetectBMP(ByteBuffer buffer,
            out uint width, out uint height)
        {
            buffer.PeekUintLE(0x12, out width);
            buffer.PeekUintLE(0x16, out height);
            return true;
        }

        private static bool DetectWEBP(ByteBuffer buffer,
            out uint width, out uint height)
        {
            buffer.PeekUshortLE(0x1A, out var w);
            width = w;
            buffer.PeekUshortLE(0x1C, out var h);
            height = h;
            return true;
        }
    }
}
