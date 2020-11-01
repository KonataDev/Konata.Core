using System;
using System.Linq;
using Konata.Msf.Packets.Oidb;

namespace Konata.Test.Tests
{
    class TestOidbCmd0x570_8 : Test
    {
        public override bool Run()
        {
            var packet = new OidbCmd0x570_8(2333333, 12345678, 600);

            Print(packet);
            Assert(Enumerable.SequenceEqual(packet.GetBytes(), new byte[]
            {
                0x08, 0xF0, 0x0A, 0x10, 0x08, 0x18, 0x00, 0x22,
                0x0F, 0x00, 0x23, 0x9A, 0x95, 0x20, 0x00, 0x01,
                0x00, 0xBC, 0x61, 0x4E, 0x00, 0x00, 0x02, 0x58
            }));

            return true;
        }
    }
}
