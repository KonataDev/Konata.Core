using System;
using System.Text;
using Konata.Core.Utils.IO;

namespace Konata.Core.Utils.Protobuf.ProtoModel;

internal class ProtoLengthDelimited : IProtoType
{
    public byte[] Value { get; set; }

    public static ProtoLengthDelimited Create(byte[] value)
        => new ProtoLengthDelimited {Value = value};

    public static ProtoLengthDelimited Create(string value)
        => new ProtoLengthDelimited {Value = Encoding.UTF8.GetBytes(value)};

    public static byte[] Serialize(ProtoLengthDelimited value)
    {
        var buffer = ByteConverter.NumberToVarint(value.Value.Length);
        var headLength = buffer.Length;

        Array.Resize(ref buffer, buffer.Length + value.Value.Length);
        Array.Copy(value.Value, 0, buffer, headLength, value.Value.Length);

        return buffer;
    }

    public override string ToString()
        => Encoding.UTF8.GetString(Value);

    public static implicit operator string(ProtoLengthDelimited value)
        => value.ToString();

    public static implicit operator byte[](ProtoLengthDelimited value)
        => value.Value;
}
