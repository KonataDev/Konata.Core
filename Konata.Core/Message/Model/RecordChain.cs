using System;
using System.IO;
using System.Linq;
using Konata.Codec.Audio;
using Konata.Codec.Audio.Codecs;
using Konata.Core.Events.Model;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Extensions;
using Konata.Core.Utils.FileFormat;

#pragma warning disable CS8509

// ReSharper disable UnusedMember.Global
// ReSharper disable RedundantAssignment
// ReSharper disable InvertIf
// ReSharper disable RedundantCaseLabel
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Message.Model;

public class RecordChain : BaseChain
{
    /// <summary>
    /// Record url (obsolete)
    /// </summary>
    [Obsolete]
    public string RecordUrl { get; }

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
        byte[] md5, string md5Str, RecordType type)
        : base(ChainType.Record, ChainMode.Singleton)
    {
        FileData = data;
        FileLength = (uint) data.Length;
        TimeSeconds = timesec;
        HashData = md5;
        FileHash = md5Str;
        RecordType = type;
        FileName = $"{md5Str}.amr";
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
            var audioType = RecordType.Silk;
            var audioData = Array.Empty<byte>();
            var audioTime = .0d;

            // Process
            switch (type)
            {
                // Amr format
                // We no need to convert it
                case FileFormat.AudioFormat.Amr:
                    audioData = audio;
                    audioType = RecordType.Amr;

                    // Estimated time
                    audioTime = audio.Length / 1607.0;

                    break;

                // Silk v3 for tx use
                case FileFormat.AudioFormat.TenSilkV3:
                    audioData = audio;
                    audioType = RecordType.Silk;
                    audioTime = FileFormat.GetSilkTime(audio, 1);
                    break;

                // Normal silk v3
                // We need to append a header 0x02
                // and remove 0xFFFF end for it
                case FileFormat.AudioFormat.SilkV3:
                {
                    audioType = RecordType.Silk;
                    audioData = new byte[] {0x02}.Concat(audio[..^2]).ToArray();
                    audioTime = FileFormat.GetSilkTime(audio);
                    break;
                }

                // Cannot convert unknown type
                case FileFormat.AudioFormat.Unknown:
                    return null;

                // Need to convert
                default:
                case FileFormat.AudioFormat.Mp3:
                case FileFormat.AudioFormat.Wav:
                {
                    using var inputStream = new MemoryStream(audio);
                    using var outputStream = new MemoryStream();
                    using var audioPipeline = new AudioPipeline
                    {
                        type switch
                        {
                            // Decode Mp3 to pcm
                            FileFormat.AudioFormat.Mp3 =>
                                new Mp3Codec.Decoder(inputStream),

                            // Decode Wav to pcm
                            FileFormat.AudioFormat.Wav =>
                                throw new NotImplementedException(),
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
                        audioType = RecordType.Silk;
                        audioData = outputStream.ToArray();
                        audioTime = audioPipeline.GetAudioTime();
                        audioPipeline.Dispose();
                    }

                    break;
                }
            }

            // Audio MD5
            var audioMd5 = audioData.Md5();
            var audioMd5Str = audioMd5.ToHex().ToUpper();

            audioTime = Math.Round(audioTime);
            if (audioTime == 0) audioTime = 1;

            return new RecordChain(audioData, (uint) audioTime,
                audioMd5, audioMd5Str, audioType);
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
    /// Not including the header 'base64://'
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static RecordChain CreateFromBase64(string base64)
        => Create(ByteConverter.UnBase64(base64));

    /// <summary>
    /// Create a record chain from url (limit 10MB)
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static RecordChain CreateFromUrl(string url)
        => Create(Utils.Network.Http.Get(url, limitLen: 1048576 * 10).Result);

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

    internal override string ToPreviewString()
        => "[语音]";
}

/// <summary>
/// Record type
/// </summary>
public enum RecordType
{
    Amr = 0,
    Silk = 1,
}
