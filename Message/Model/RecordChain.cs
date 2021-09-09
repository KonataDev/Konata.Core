using System;
using System.IO;
using System.Linq;
using Konata.Codec.Audio;
using Konata.Codec.Audio.Codecs;
using Konata.Core.Events.Model;
using Konata.Core.Utils.Crypto;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.FileFormat;

// ReSharper disable RedundantAssignment
// ReSharper disable InvertIf
// ReSharper disable RedundantCaseLabel
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message.Model
{
    public class RecordChain : BaseChain
    {
        [Obsolete] public string RecordUrl { get; }

        /// <summary>
        /// Self uin
        /// </summary>
        public uint SelfUin { get; internal set; }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// File hash
        /// </summary>
        public string FileHash { get; }

        /// <summary>
        /// File hash data
        /// </summary>
        public byte[] HashData { get; }

        /// <summary>
        /// File data
        /// </summary>
        public byte[] FileData { get; }

        /// <summary>
        /// File length
        /// </summary>
        public uint FileLength { get; }

        /// <summary>
        /// Record duration
        /// </summary>
        public uint TimeSeconds { get; }

        /// <summary>
        /// Record type
        /// </summary>
        public RecordType RecordType { get; }

        /// <summary>
        /// Ptt upload information
        /// </summary>
        internal PttUpInfo PttUpInfo { get; private set; }

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
            FileName = $"{md5str}.amr";
        }

        /// <summary>
        /// Set Upload id
        /// </summary>
        /// <param name="selfUin"></param>
        /// <param name="info"></param>
        internal void SetPttUpInfo(uint selfUin, PttUpInfo info)
        {
            SelfUin = selfUin;
            PttUpInfo = info;
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
        /// Create a record chain
        /// </summary>
        /// <param name="audio"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static RecordChain Create(byte[] audio)
        {
            // Detect image type
            if (FileFormat.DetectAudio(audio, out var type))
            {
                var audioType = RecordType.SILK;
                var audioData = Array.Empty<byte>();
                var audioTime = 0U;
                
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

                    // Cannot convert unknown type
                    case FileFormat.AudioFormat.UNKNOWN:
                        return null;

                    // Need to convert
                    default:
                    case FileFormat.AudioFormat.MP3:
                    case FileFormat.AudioFormat.WAV:
                    {
                        using var inputStream = new MemoryStream(audio);
                        using var outputStream = new MemoryStream();
                        using var audioPipeline = new AudioPipeline
                        {
                            type switch
                            {
                                // Decode Mp3 to pcm
                                FileFormat.AudioFormat.MP3 =>
                                    new Mp3Codec.Decoder(inputStream),

                                // Decode Wav to pcm
                                FileFormat.AudioFormat.WAV =>
                                    throw new NotImplementedException()
                            },

                            // Resample audio to silkv3
                            new AudioResampler(AudioInfo.SilkV3()),

                            // Encode pcm to silkv3
                            new SilkV3Codec.Encoder(),

                            // Output stream
                            outputStream
                        };

                        // Start pipeline
                        if (!audioPipeline.Start().Result) return null;
                        {
                            // Set audio information
                            audioType = RecordType.SILK;
                            audioData = outputStream.ToArray();
                            audioTime = (uint)audioPipeline.GetAudioTime();
                        }

                        break;
                    }
                }

                // Audio MD5
                var audioMD5 = new Md5Cryptor().Encrypt(audioData);
                var audioMD5Str = ByteConverter.Hex(audioMD5).ToUpper();

                return new RecordChain(audioData, audioTime,
                    audioMD5, audioMD5Str, audioType);
            }

            return null;
        }

        /// <summary>
        /// Create a record chain
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static RecordChain CreateFromFile(string filepath)
        {
            // File not exist
            if (!File.Exists(filepath))
                throw new FileNotFoundException(filepath);

            return Create(File.ReadAllBytes(filepath));
        }

        /// <summary>
        /// Create a record chain from plain base64 <br />
        /// Not incuding the header 'base64://'
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static RecordChain CreateFromBase64(string base64)
            => Create(ByteConverter.UnBase64(base64));

        /// <summary>
        /// Parse the code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static RecordChain Parse(string code)
        {
            var args = GetArgs(code);
            {
                var file = args["file"];

                // Create from base64
                if (file.StartsWith("base64://"))
                {
                    return CreateFromBase64(file[9..file.Length]);
                }

                // Create from local file
                if (File.Exists(file))
                {
                    return CreateFromFile(file);
                }
            }

            // Ignore
            return null;
        }

        public override string ToString()
            => "[KQ:record," +
               $"file={FileName}]";
    }

    public enum RecordType
    {
        AMR = 0,
        SILK = 1,
    }
}
