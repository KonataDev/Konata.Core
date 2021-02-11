using System;

using Konata.Utils.IO;

namespace Konata.Utils.Protobuf.ProtoModel
{
    public class ProtoVarInt : IProtoType
    {
        public long Value { get; set; }

        public static ProtoVarInt Create(byte[] value)
            => new ProtoVarInt { Value = ByteConverter.VarintToNumber(value) };

        public static ProtoVarInt Create(long value)
            => new ProtoVarInt { Value = value };

        public static byte[] Serialize(ProtoVarInt value)
            => ByteConverter.NumberToVarint(value.Value);
    }
}
