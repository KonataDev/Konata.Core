using Konata.Core.Utils.IO;

namespace Konata.Core.Utils.Protobuf.ProtoModel;

internal class ProtoVarInt : IProtoType
{
    public long Value { get; set; }

    public static ProtoVarInt Create(byte[] value)
        => new ProtoVarInt {Value = ByteConverter.VarintToNumber(value)};

    public static ProtoVarInt Create(long value)
        => new ProtoVarInt {Value = value};

    public static byte[] Serialize(ProtoVarInt value)
        => ByteConverter.NumberToVarint(value.Value);

    public static implicit operator long(ProtoVarInt value)
        => value.Value;

    public static implicit operator uint(ProtoVarInt value)
        => (uint) value.Value;

    public static implicit operator int(ProtoVarInt value)
        => (int) value.Value;
}
