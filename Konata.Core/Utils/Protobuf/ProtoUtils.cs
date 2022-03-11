using System;
using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf.ProtoModel;

namespace Konata.Core.Utils.Protobuf
{
    internal static class ProtoUtils
    {
        internal static ProtoType VarintGetPbType(byte[] variant)
        {
            return (ProtoType)(ByteConverter.VarintToNumber(variant) & 7);
        }
    }
}
