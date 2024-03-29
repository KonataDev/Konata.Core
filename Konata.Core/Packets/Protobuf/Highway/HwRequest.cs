﻿using Konata.Core.Utils.IO;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Packets.Protobuf.Highway;

internal static class HwRequest
{
    public static byte[] Create(PicUp picup, byte[] payload = null)
    {
        var buffer = new ByteBuffer();
        var pbhead = ProtoTreeRoot.Serialize(picup).GetBytes();
        {
            buffer.PutByte(0x28);
            {
                // PBhead length
                buffer.PutUintBE((uint) pbhead.Length);

                // Payload length
                buffer.PutUintBE((uint) (payload?.Length ?? 0));

                // PBhead data
                buffer.PutBytes(pbhead, ByteBuffer.Prefix.None);

                // Payload data
                if (payload != null)
                {
                    buffer.PutBytes(payload, ByteBuffer.Prefix.None);
                }
            }
            buffer.PutByte(0x29);
        }

        return buffer.GetBytes();
    }
}
