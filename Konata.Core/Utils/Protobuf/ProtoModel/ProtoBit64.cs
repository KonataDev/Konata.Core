﻿using System;
using Konata.Core.Utils.IO;

namespace Konata.Core.Utils.Protobuf.ProtoModel;

internal class ProtoBit64 : IProtoType
{
    public long Value { get; set; }

    public static ProtoBit64 Create(byte[] value)
        => new ProtoBit64 {Value = ByteConverter.BytesToInt64(value, 0, Endian.Little)};

    public static ProtoBit64 Create(long value)
        => new ProtoBit64 {Value = value};

    public static byte[] Serialize(ProtoBit64 value)
        => ByteConverter.Int64ToBytes(value.Value, Endian.Little);
}
