using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Packets.Protobuf.Highway;

internal class HwResponse : ProtoTreeRoot
{
    public byte Version { get; private set; }

    public uint PeerUin { get; private set; }

    public string Command { get; private set; }

    public uint Sequence { get; private set; }

    public uint RetryTimes { get; private set; }

    public uint CommandId { get; private set; }

    public uint LocaleId { get; private set; }

    private HwResponse(byte[] pbhead)
        : base(pbhead, true)
    {
        GetTree("0A", r =>
        {
            Version = (byte) r.GetLeafVar("08");

            // Uin string
            PeerUin = uint.Parse(r.GetLeafString("12"));

            // Command
            Command = r.GetLeafString("1A");

            // Sequence
            Sequence = (uint) r.GetLeafVar("20");

            // Retry times
            RetryTimes = (uint) r.GetLeafVar("28");

            // Command id
            CommandId = (uint) r.GetLeafVar("40");

            // Locale id
            LocaleId = (uint) r.GetLeafVar("50");
        });
    }

    /// <summary>
    /// Parse a response
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    internal static HwResponse Parse(byte[] data)
    {
        var buffer = new ByteBuffer(data);
        {
            buffer.EatBytes(1);

            // PBhead length
            buffer.TakeUintBE(out var pbheadLen);

            // Payload length
            buffer.EatBytes(4);

            // PBhead data
            buffer.TakeBytes(out var pbhead, pbheadLen);

            buffer.EatBytes(1);

            return new(pbhead);
        }
    }
}
