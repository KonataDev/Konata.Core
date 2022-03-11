using Konata.Core.Utils.IO;

// ReSharper disable ConvertIfStatementToSwitchStatement

namespace Konata.Core.Utils.FileFormat
{
    public static partial class FileFormat
    {
        public enum AudioFormat
        {
            UNKNOWN,
            WAV,
            MP3,
            SILKV3,
            TENSILKV3,
            AMR,
        }

        /// <summary>
        /// Detect audio type
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool DetectAudio(byte[] data, out AudioFormat type)
        {
            // Read first 16 bytes
            var buffer = new ByteBuffer(data[..16]);
            {
                buffer.PeekUintBE(0, out var value);

                // SILKV3
                //  #  !  S  I  L  K
                // +--+--+--+--+--+--+
                // |23|21|53|49|4C|4B|
                // +--+--+--+--+--+--+
                // 0           4     6  
                if (value == 0x23215349)
                {
                    type = AudioFormat.SILKV3;
                    return true;
                }

                // TENSILKV3
                //     #  !  S  I  L  K
                // +--+--+--+--+--+--+--+
                // |02|23|21|53|49|4C|4B|
                // +--+--+--+--+--+--+--+
                // 0           4        7  
                if (value == 0x02232153)
                {
                    type = AudioFormat.TENSILKV3;
                    return true;
                }

                // AMR
                //     #  !  A  M  R
                // +--+--+--+--+--+--+
                // |02|23|21|41|4D|52|
                // +--+--+--+--+--+--+
                // 0           4     6
                if (value == 0x2321414D)
                {
                    type = AudioFormat.AMR;
                    return true;
                }

                // WAV
                //  R  I  F  F  .  .  .  .  W  A  V  E  f  m  t
                // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                // |52|49|46|46|00|00|00|00|57|41|56|45|66|6D|74|
                // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
                // 0           4           8           12       15
                if (value == 0x52494646 &&
                    buffer.PeekUintBE(8, out _) == 0x57415645)
                {
                    type = AudioFormat.WAV;
                    return true;
                }

                // MP3
                //  I  D  3
                // +--+--+--+
                // |49|44|33|
                // +--+--+--+
                // 0        3
                if (value >> 8 == 0x494433)
                {
                    type = AudioFormat.MP3;
                    return true;
                }
            }

            type = AudioFormat.UNKNOWN;
            return false;
        }
    }
}
