using System;
using System.Linq;
using Konata.AvCodec;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.FileFormat;

// ReSharper disable RedundantAssignment
// ReSharper disable RedundantCaseLabel
// ReSharper disable NotAccessedVariable
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message.Model
{
    public class RecordChain : BaseChain
    {
        public string RecordUrl { get; }

        public string FileName { get; }

        public string FileHash { get; }

        public byte[] HashData { get; }

        public byte[] FileData { get; }

        public uint FileLength { get; }

        public uint TimeSeconds { get; }

        public RecordType RecordType { get; }

        private RecordChain(string url, string fileName, string fileHash)
            : base(ChainType.Record, ChainMode.Singleton)
        {
            RecordUrl = url;
            FileName = fileName;
            FileHash = fileHash;
        }

        private RecordChain(byte[] data, uint timesec,
            byte[] md5, string md5str, RecordType type)
            : base(ChainType.Record, ChainMode.Singleton)
        {
            FileData = data;
            FileLength = (uint) data.Length;
            TimeSeconds = timesec;
            HashData = md5;
            FileHash = md5str;
            RecordType = type;
            FileName = md5str;
        }

        /// <summary>
        /// Create a record chain
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="fileHash"></param>
        /// <returns></returns>
        internal static RecordChain Create(string url,
            string fileName, string fileHash)
        {
            return new(url, fileName, fileHash);
        }

        /// <summary>
        /// Create an audio chain
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static RecordChain Create(byte[] audio)
        {
            // Detect image type
            if (FileFormat.DetectAudio(audio, out var type,
                out var timeSec, out var sampleRate, out var channel))
            {
                byte[] audioData = null;
                RecordType audioType = RecordType.SILK;

                // Process
                switch (type)
                {
                    // Amr format
                    // We no need to convert it
                    case FileFormat.AudioFormat.AMR:
                        audioData = audio;
                        audioType = RecordType.AMR;
                        break;

                    // Silk v3 for tx use
                    case FileFormat.AudioFormat.TENSILKV3:
                        audioData = audio;
                        audioType = RecordType.SILK;
                        break;

                    // Normal silk v3
                    // We need to append a header 0x02
                    // and remove 0xFFFF end for it
                    case FileFormat.AudioFormat.SILKV3:
                    {
                        audioType = RecordType.SILK;
                        audioData = new byte[] {0x02}
                            .Concat(audio[..^2]).ToArray();
                        break;
                    }

                    // Need to convert
                    default:
                    case FileFormat.AudioFormat.MP3:
                    case FileFormat.AudioFormat.WAV:
                    case FileFormat.AudioFormat.UNKNOWN:
                    {
                        var pcmData = Array.Empty<byte>();
                        var silkData = Array.Empty<byte>();

                        // TODO:
                        // Convert audio format to silk

                        switch (type)
                        {
                            // Decode Mp3 to pcm
                            case FileFormat.AudioFormat.MP3:
                                throw new NotImplementedException();

                            // Decode Wav to pcm
                            case FileFormat.AudioFormat.WAV:
                                throw new NotImplementedException();

                            // Cannot convert unknown type
                            case FileFormat.AudioFormat.UNKNOWN:
                                return null;
                        }

                        // TODO:
                        // Resample the pcm to
                        // signed 16bit mono 24kHz format 
                        // if (channel != 1 || sampleRate != 24000)
                        // {
                        // }

                        // Convert pcm data to silkv3
                        if (pcmData.Length != 0 && SilkCodec
                            .Encode(pcmData, 24000, out silkData).Result)
                        {
                            // Set audio information
                            audioData = silkData;
                            audioType = RecordType.SILK;
                        }

                        break;
                    }
                }

                // Audio MD5
                var audioMD5 = new Md5Cryptor().Encrypt(audioData);
                var audioMD5Str = ByteConverter.Hex(audioMD5).ToUpper();

                return new RecordChain(audioData, (uint) timeSec,
                    audioMD5, audioMD5Str, audioType);
            }

            return null;
        }

        /// <summary>
        /// Create an record chain from plain base64 <br />
        /// Not incuding the header 'base64://'
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static RecordChain CreateFromBase64(string base64)
        {
            return Create(ByteConverter.UnBase64(base64));
        }

        /// <summary>
        /// Parse the code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static RecordChain Parse(string code)
        {
            return null;
        }

        public override string ToString()
            => $"[KQ:record," +
               $"file={FileName}]";
    }

    public enum RecordType
    {
        AMR = 1005,
        SILK = 1002,
    }
}
