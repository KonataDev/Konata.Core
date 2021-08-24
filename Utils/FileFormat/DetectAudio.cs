using System;
using Konata.Core.Utils.IO;

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
        /// <param name="timesec"></param>
        /// <param name="samplerate"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static bool DetectAudio(byte[] data, out AudioFormat type,
            out int timesec, out int samplerate, out int channel)
        {
            var buffer = new ByteBuffer(data);
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
                    return DetectSilkV3(buffer, out timesec,
                        out samplerate, out channel);
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
                    return DetectSilkV3(buffer, out timesec,
                        out samplerate, out channel);
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
                    return DetectWav(buffer, out timesec,
                        out samplerate, out channel);
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
                    return DetectMp3(buffer, out timesec,
                        out samplerate, out channel);
                }

                // TODO:
                // AMR
            }

            type = AudioFormat.UNKNOWN;
            timesec = samplerate = channel = 0;
            return false;
        }

        private static bool DetectSilkV3(ByteBuffer buffer,
            out int timesec, out int samplerate, out int channel)
        {
            // Fixed 1chn and 24kHz
            channel = 1;
            samplerate = 24000;

            // Fake (?
            timesec = (int) Math.Ceiling
                (buffer.RemainLength / 2886.923);
            return true;
        }

        private static bool DetectWav(ByteBuffer buffer,
            out int timesec, out int samplerate, out int channel)
        {
            // |ChunkSize  |Fmt  |Chn  |SampleRate |ByteRate   |Align|Bits |d  a  t  a |ChunkSize
            // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // |10|00|00|00|01|00|02|00|22|56|00|00|88|58|01|00|04|00|10|00|64|61|74|61|00|00|00|00|
            // +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            // 16          20          24          28          32          36          40          43         

            // Only supported PCM
            timesec = samplerate = channel = 0;
            if (buffer.PeekUintBE(20, out _) != 0x1000) return false;

            var bps = buffer.PeekUintBE(34, out _);
            var shunksize = buffer.PeekUintBE(40, out _);

            channel = (int) buffer.PeekUshortLE(22, out _);
            samplerate = (int) buffer.PeekUintLE(24, out _);
            timesec = (int) Math.Ceiling((((double) shunksize / channel / ((int) (bps / 8))) / samplerate));
            return true;
        }

        private static bool DetectMp3(ByteBuffer buffer,
            out int timesec, out int samplerate, out int channel)
        {
            // TODO:
            timesec = samplerate = channel = 0;
            return false;
        }
    }
}
