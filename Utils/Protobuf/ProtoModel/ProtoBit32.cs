using System;

using Konata.Utils.IO;

namespace Konata.Utils.Protobuf.ProtoModel
{
    public class ProtoBit32 : IProtoType
    {
        public int Value { get; set; }

        public static ProtoBit32 Create(byte[] value)
            => new ProtoBit32 { Value = ByteConverter.BytesToInt32(value, 0, Endian.Little) };

        public static ProtoBit32 Create(int value)
            => new ProtoBit32 { Value = value };

        public static byte[] Serialize(ProtoBit32 value)
            => ByteConverter.Int32ToBytes(value.Value, Endian.Little);
    }
}

