using System;
using System.Linq;
using Konata.Msf.Packets.Oidb;

namespace Konata.Test.Tests
{
    class TestOidbCmd0x88d_0 : Test
    {
        public override bool Run()
        {
            var packet = new OidbCmd0x88d_0(2333);

            Print(packet);
            Assert(Enumerable.SequenceEqual(packet.GetBytes(), new byte[]
            {
                0x08, 0x8D, 0x11, 0x10, 0x00, 0x22, 0x17, 0x08,
                0x94, 0x84, 0xAF, 0x5F, 0x12, 0x10, 0x08, 0x9D,
                0x12, 0x12, 0x0B, 0x40, 0x00, 0x88, 0x03, 0x00,
                0xE0, 0x04, 0x00, 0xD0, 0x05, 0x00
            }));

            return true;
        }
    }
}
