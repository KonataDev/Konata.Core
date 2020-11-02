using Konata.Library.IO;
using System;

namespace Konata.Test.Tests
{
    class TestByteBuffer : Test
    {
        public override bool Run()
        {
            var buffer = new ByteBuffer();
            {
                buffer.PutBytes(new byte[0],
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
            }
            buffer.TakeUintBE(out var length);

            Assert(length == 4);
            return true;
        }
    }
}
