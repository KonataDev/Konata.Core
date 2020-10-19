using System;
using Konata.Library.IO;

namespace Konata.Library.Protobuf
{
    internal static class ProtoUtils
    {
        internal static ProtoType VarintGetPbType(byte[] variant)
        {
            return (ProtoType)(ByteConverter.VarintToNumber(variant) & 7);
        }
    }
}
