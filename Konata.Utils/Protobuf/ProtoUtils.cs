using System;
using Konata.Utils.IO;

namespace Konata.Utils.Protobuf
{
    internal static class ProtoUtils
    {
        internal static ProtoType VarintGetPbType(byte[] variant)
        {
            return (ProtoType)(ByteConverter.VarintToNumber(variant) & 7);
        }
    }
}
