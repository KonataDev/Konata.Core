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
                buffer.PutString("Test",
                    ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);
            }
            buffer.TakeUintBE(out var length);
            buffer.TakeString(out var outstr,
                ByteBuffer.Prefix.Uint32 | ByteBuffer.Prefix.WithPrefix);

            Assert(length == 4);
            Assert(outstr == "Test");

            return true;
        }
    }
}
