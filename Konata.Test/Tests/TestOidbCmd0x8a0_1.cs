using System;
using System.Linq;
using Konata.Msf.Packets.Oidb;

namespace Konata.Test.Tests
{
    class TestOidbCmd0x8a0_1 : Test
    {
        public override bool Run()
        {
            var packet = new OidbCmd0x8a0_1(23333, 2333, true);

            Print(packet);
            Assert(Enumerable.SequenceEqual(packet.GetBytes(), new byte[]
            {
                0x08, 0xA0, 0x11, 0x10, 0x01, 0x22, 0x09, 0x08,
                0xA5, 0xB6, 0x01, 0x18, 0x9D, 0x12, 0x20, 0x01
            }));

            return true;
        }
    }
}
