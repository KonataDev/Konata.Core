using System;
using System.Linq;
using Konata.Packets.Oidb;

namespace Konata.Test.Tests
{
    class TestOidbCmd0x899_0 : Test
    {
        public override bool Run()
        {
            var packet = new OidbCmd0x899_0(23333);

            Print(packet);
            Assert(Enumerable.SequenceEqual(packet.GetBytes(), new byte[]
            {
                0x08, 0x99, 0x11, 0x10, 0x00, 0x18, 0x00, 0x22,
                0x0F, 0x08, 0xA5, 0xB6, 0x01, 0x10, 0x00, 0x18,
                0x02, 0x2A, 0x05, 0x08, 0x00, 0x90, 0x01, 0x01
            }));

            return true;
        }
    }
}
