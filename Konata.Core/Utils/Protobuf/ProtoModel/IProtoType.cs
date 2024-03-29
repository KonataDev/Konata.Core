﻿using System;

namespace Konata.Core.Utils.Protobuf.ProtoModel;

internal enum ProtoType : byte
{
    VarInt = 0,

    Bit64 = 1,

    LengthDelimited = 2,

    [Obsolete] StartGroup = 3,

    [Obsolete] EndGroup = 4,

    Bit32 = 5,
}

internal interface IProtoType
{
}
