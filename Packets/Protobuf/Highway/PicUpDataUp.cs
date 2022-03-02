using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf.Highway;

internal class PicUpDataUp : PicUp
{
    public const string Command = "PicUp.DataUp";

    /// <summary>
    /// Data up
    /// </summary>
    /// <param name="cmdId"></param>
    /// <param name="peerUin"></param>
    /// <param name="sequence"></param>
    /// <param name="ticket"></param>
    /// <param name="fileSize"></param>
    /// <param name="fileMD5"></param>
    /// <param name="chunkOffset"></param>
    /// <param name="chunkSize"></param>
    /// <param name="chunkMD5"></param>
    /// <param name="request"></param>
    public PicUpDataUp(CommandId cmdId, uint peerUin, int sequence, byte[] ticket,
        int fileSize, byte[] fileMD5, int chunkOffset, int chunkSize, byte[] chunkMD5, ProtoTreeRoot request = null)
        : base(Command, cmdId, peerUin, sequence)
    {
        AddTree("12", w =>
        {
            // File size
            w.AddLeafVar("10", fileSize);

            // Chunk offset
            w.AddLeafVar("18", chunkOffset);

            // Chunk size
            w.AddLeafVar("20", chunkSize);

            // Service ticket
            w.AddLeafBytes("32", ticket);

            // Chunk md5
            w.AddLeafBytes("42", chunkMD5);

            // File md5
            w.AddLeafBytes("4A", fileMD5);
        });

        // Additional request body
        if (request != null) AddTree("1A", request);
    }
}
